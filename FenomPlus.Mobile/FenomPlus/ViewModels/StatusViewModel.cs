 using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FenomPlus.Controls;
using FenomPlus.Services.DeviceService.Concrete;
using FenomPlus.Views;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Essentials;
using Xamarin.Forms;

// Note: Shared by DeviceStatusHubView and TitleContentView

namespace FenomPlus.ViewModels
{
    public partial class StatusViewModel : BaseViewModel
    {
        private const int TimerIntervalMilliseconds = 1000;     // Bluetooth Check every one second
        private int RequestNewStatusInterval = 15;        // New DeviceInfo and EnvironmentInfo every 15 seconds

        public StatusButtonViewModel SensorViewModel = new StatusButtonViewModel();
        public StatusButtonViewModel DeviceViewModel = new StatusButtonViewModel();
        public StatusButtonViewModel QualityControlViewModel = new StatusButtonViewModel();
        public StatusButtonViewModel BluetoothViewModel = new StatusButtonViewModel();
        public StatusButtonViewModel HumidityViewModel = new StatusButtonViewModel();
        public StatusButtonViewModel PressureViewModel = new StatusButtonViewModel();
        public StatusButtonViewModel TemperatureViewModel = new StatusButtonViewModel();
        public StatusButtonViewModel BatteryViewModel = new StatusButtonViewModel();

        // Used in the titlebar status region
        [ObservableProperty]
        public string _sensorBarIcon;
        [ObservableProperty]
        public string _deviceBarIcon;
        [ObservableProperty]
        public string _qcBarIcon;
        [ObservableProperty]
        public string _bluetoothBarIcon;
        [ObservableProperty]
        public string _humidityBarIcon;
        [ObservableProperty]
        public string _pressureBarIcon;
        [ObservableProperty]
        public string _temperatureBarIcon;
        [ObservableProperty]
        public string _batteryBarIcon;

        [ObservableProperty] private bool _batteryBarIconVisible;

        [ObservableProperty]
        private bool _sensorBarIconVisible;

        [ObservableProperty]
        private bool _deviceBarIconVisible;

        [ObservableProperty]
        private bool _qcBarIconVisible;

        [ObservableProperty]
        private bool _pressureBarIconVisible;

        [ObservableProperty]
        private bool _humidityBarIconVisible;

        [ObservableProperty]
        private bool _temperatureBarIconVisible;

        [ObservableProperty]
        private bool _bluetoothConnected;

        private readonly Timer BluetoothStatusTimer;

        private bool _DeviceNotFound;

        public StatusViewModel()
        {
            VersionTracking.Track();

            BluetoothViewModel.Header = "Bluetooth";
            SensorViewModel.Header = "Sensor";
            DeviceViewModel.Header = "Device";
            QualityControlViewModel.Header = "Quality Control";
            HumidityViewModel.Header = "Humidity";
            PressureViewModel.Header = "Pressure";
            TemperatureViewModel.Header = "Temperature";
            BatteryViewModel.Header = "Battery";

            BluetoothConnected = false;

            BluetoothStatusTimer = new Timer(TimerIntervalMilliseconds);
            BluetoothStatusTimer.Elapsed += BluetoothCheck;
            BluetoothStatusTimer.Start();

            MessagingCenter.Subscribe<DevicePowerOnViewModel>(this, "DeviceNotFound", async (sender) => {
                await Task.Run(() =>
                {
                    _DeviceNotFound = true;
                });
            });
        }

        private bool CheckDeviceConnection()
        {
            // Don't use Services.BleHub.IsConnected() or it will try to reconnect - we just want current connection status
            return Services?.DeviceService?.Current?.Connected ?? false;
        }

        private int BluetoothCheckCount = 0;
        /// <summary>
        /// When app switch from disconnection to connection, it need to RefreshStatusAsync 10 times in an interval of 1 seconds 
        /// else, though BTLE connected, the BTLE icon keeps red for 20s
        /// </summary>
        private int counter_when_switch_from_disconnection = 10;
        private  void BluetoothCheck(object sender, ElapsedEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                // Note:  All device status parameters are conditional on the bluetooth connection
                BluetoothConnected = CheckDeviceConnection();

                if (BluetoothConnected)
                {
                    if (counter_when_switch_from_disconnection > 0) --counter_when_switch_from_disconnection;
                    else
                    {
                        var currentPage = App.GetCurrentPage();
                        if (currentPage is DeviceStatusHubView)
                        {
                            RequestNewStatusInterval = 3;
                        }
                        else
                        {
                            if (currentPage is DevicePowerOnView)  // ToDo: Only needed because viewmodels never die
                            {
                                // Only navigate if during startup
                                await Services.Navigation.DashboardView();
                            }
                            RequestNewStatusInterval = 20;
                        }
                        BluetoothCheckCount++;

                        if (BluetoothCheckCount >= RequestNewStatusInterval)
                            BluetoothCheckCount = 0;
                    }
                }
                else
                {
                    counter_when_switch_from_disconnection = 5;
                    BluetoothCheckCount = 0; // Reset counter
                    if (Services.DeviceService.Discovering)
                    {
                        if (App.GetCurrentPage() is not DevicePowerOnView)
                        {
                            await Services.Navigation.DevicePowerOnView();
                        }
                    }
                    // else  // Not Discovering, Not BluetoothConnected, could be found, could be DeviceNotFound
                    else if (_DeviceNotFound) // Not Discovering, Not BluetoothConnected, could be DeviceDiscovered , could be DeviceNotFound
                    {
                        if (App.GetCurrentPage() is not DashboardView)
                        {
                            await Services.Navigation.DashboardView();
                        }
                    }
                }

                if (BluetoothCheckCount == 0 && Services.Cache.TestType == Enums.TestTypeEnum.None) 
                    await RefreshStatusAsync();
            });
        }

        private bool RefreshInProgress = false;

        public async Task RefreshStatusAsync()
        {
            //Debugger.Break();

            // To early to get status or don't update environmental properties during test - Important - DO NOT REMOVE!
            if (Services.DeviceService.Current != null && (!BluetoothConnected ||
                                                           RefreshInProgress ||
                                                           Services.DeviceService.Current.BreathTestInProgress ||
                                                           Services.DeviceService.Current.EnvironmentalInfo == null))
            {
                await Task.Delay(1);
                return;
            }


            UpdateVersionNumbers();
            UpdateBluetooth();
            UpdateDevice(Services.Cache.DeviceExpireDate);
            UpdateQualityControlExpiration(7);

            await Services.DeviceService.Current.RequestEnvironmentalInfo();
            RefreshInProgress = true;

            UpdateSensor();
            UpdateBattery();
            UpdatePressure();
            UpdateHumidity();
            UpdateTemperature();
            RefreshInProgress = false;
        }

        public void UpdateVersionNumbers()
        {
            if (BluetoothConnected)
            {
                DeviceSerialNumber = $"Device Serial Number ({Services.DeviceService.Current.DeviceSerialNumber})";
                DeviceFirmwareVersion = $"Firmware ({Services.DeviceService.Current.Firmware})";
            }
            else
            {
                DeviceSerialNumber = string.Empty;
                DeviceFirmwareVersion = string.Empty;
            }
        }

        public void UpdateDevice(DateTime expirationDate)
        {
            if (!BluetoothConnected)
            {
                DeviceBarIconVisible = false;
                DeviceViewModel.ImagePath = "device_red.png";
                DeviceViewModel.Label = string.Empty;
                DeviceViewModel.Value = string.Empty;
                DeviceViewModel.ButtonText = string.Empty;
                DeviceViewModel.Description = string.Empty;
                return;
            }

            //// ToDo: Remove hard coded value
            //expirationDate = DateTime.Now.AddYears(5).AddDays(-10);
            ////expirationDate = DateTime.Now.AddDays(5); 

            //int daysRemaining = (expirationDate > DateTime.Now) ? (int)(expirationDate - DateTime.Now).TotalDays : 0;

            int deviceLifeRemaining = Services.DeviceService.Current.DeviceLifeRemaining;
            deviceLifeRemaining = deviceLifeRemaining < 0 ? -1 * deviceLifeRemaining : deviceLifeRemaining;



            DeviceViewModel.ButtonText = "Order";

            if (deviceLifeRemaining <= Constants.DeviceExpired)
            {
                DeviceBarIconVisible = true;
                DeviceBarIcon = "wo_device_red.png";
                DeviceViewModel.ImagePath = "device_red.png";
                DeviceViewModel.ValueColor = Color.Red;
                DeviceViewModel.Description = "The device has reached the end of its lifespan. Contact Customer Service for a replacement."; DeviceViewModel.Value = $"{deviceLifeRemaining}";
                DeviceViewModel.Label = "Days Left";
            }
            else if (deviceLifeRemaining <= Constants.DeviceWarning60Days)
            {
                DeviceBarIconVisible = true;
                DeviceBarIcon = "_3x_wo_device_yellow.png";
                DeviceViewModel.ImagePath = "device_yellow.png";
                DeviceViewModel.ValueColor = Color.FromHex("#333");
                DeviceViewModel.Description = "The device will expire in less than 60 days. Contact Customer Service.";
                DeviceViewModel.Value = $"{deviceLifeRemaining}";
                DeviceViewModel.Label = "Days Left";
            }
            else
            {
                DeviceBarIconVisible = false;
                DeviceViewModel.ImagePath = "device_green_100.png";
                DeviceViewModel.ValueColor = Color.FromHex("#333");
                DeviceViewModel.Description = $"The device has {deviceLifeRemaining} days remaining before it has to be replaced.";
                DeviceViewModel.Value = $"{deviceLifeRemaining / 30}";
                DeviceViewModel.Label = "Months Left";
            }
        }

        public void UpdateSensor()
        {
            if (!BluetoothConnected || Services.DeviceService.Current == null)
            {
                SensorBarIconVisible = false;

                SensorViewModel.ImagePath = "sensor_red.png";
                SensorViewModel.Label = string.Empty;
                SensorViewModel.Value = string.Empty;
                SensorViewModel.ButtonText = string.Empty;
                SensorViewModel.Description = string.Empty;
                return;
            }
            if (Services.DeviceService.Current != null && Services.DeviceService.Current.SensorExpireDate != new DateTime(1, 1, 1))
            {
                // ToDo: Remove hard coded value
                int month = (Services.DeviceService.Current.DeviceInfo.SensorExpDateMonth);
                int day = (Services.DeviceService.Current.DeviceInfo.SensorExpDateDay);
                int year = (Services.DeviceService.Current.DeviceInfo.SensorExpDateYear);

                // TODO
                // temp fix: the status view should not be displayed until we have valid values
                //           until that's done, give some valid values...
                if (month == 0 || day == 0 || year == 0)
                {
                    month = 01;
                    day = 02;
                    year = 03;
                }

                DateTime expirationDate = new DateTime(year, month, day);

                int daysRemaining = (expirationDate > DateTime.Now) ? (int)(expirationDate - DateTime.Now).TotalDays : 0;

                
                SensorViewModel.ButtonText = "Order";

                if (daysRemaining <= Constants.SensorExpired)
                {
                    SensorBarIconVisible = true;
                    // low
                    SensorBarIcon = "wo_sensor_red.png";
                    SensorViewModel.ImagePath = "sensor_red.png";
                    SensorViewModel.ValueColor = Color.Red;
                    SensorViewModel.Description = "The nitric oxide sensor has expired. Replace it with a new sensor.";
                    SensorViewModel.Value = $"{daysRemaining}";
                    SensorViewModel.Label = "Days Left";
                }
                else if (Services.DeviceService.Current?.ErrorStatusInfo.ErrorCode == Constants.NoSensorMissing)
                {
                    SensorBarIconVisible = true;
                    // error
                    SensorBarIcon = "wo_sensor_red.png";
                    SensorViewModel.ImagePath = "sensor_red.png";
                    SensorViewModel.ValueColor = Color.Red;
                    SensorViewModel.Description = "Nitrous Oxide Sensor is missing.  Install a F150 sensor.";
                    SensorViewModel.Value = $"{daysRemaining}";
                    SensorViewModel.Label = "Days Left";
                }
                else if (Services.DeviceService.Current?.ErrorStatusInfo.ErrorCode == Constants.NoSensorCommunicationFailed)
                {
                    SensorBarIconVisible = true;
                    // error
                    SensorBarIcon = "wo_sensor_red.png";
                    SensorViewModel.ImagePath = "sensor_red.png";
                    SensorViewModel.ValueColor = Color.Red;
                    SensorViewModel.Description = "Nitrous Oxide Sensor communication failed.";
                    SensorViewModel.Value = $"{daysRemaining}";
                    SensorViewModel.Label = "Days Left";
                }
                else if (daysRemaining <= Constants.SensorWarning60Days)
                {
                    SensorBarIconVisible = true;
                    // warning
                    SensorBarIcon = "wo_sensor_yellow.png";
                    SensorViewModel.ImagePath = "sensor_yellow.png";
                    SensorViewModel.ValueColor = Color.FromHex("#333");
                    SensorViewModel.Description = "The sensor will expire in less than 60 days. Contact Customer Service.";
                    SensorViewModel.Value = $"{daysRemaining}";
                    SensorViewModel.Label = "Days Left";
                }
                else
                {
                    SensorBarIconVisible = false;
                    SensorViewModel.ImagePath = "sensor_green.png";
                    SensorViewModel.ValueColor = Color.FromHex("#333");
                    SensorViewModel.Description = $"Sensor is good for another {daysRemaining} days.";
                    SensorViewModel.Value = $"{daysRemaining / 30}";
                    SensorViewModel.Label = "Months Left";
                }
            }
            
        }

        public void UpdateQualityControlExpiration(int value)
        {
            if (!BluetoothConnected)
            {
                QcBarIconVisible = false;

                QualityControlViewModel.ImagePath = "quality_control_red.png";
                QualityControlViewModel.Label = string.Empty;
                QualityControlViewModel.Value = string.Empty;
                QualityControlViewModel.ButtonText = string.Empty;
                QualityControlViewModel.Description = string.Empty;
                return;
            }

            QualityControlViewModel.Value = $"{value}";
            QualityControlViewModel.Label = "Days Left";
            QualityControlViewModel.ButtonText = "Settings";

            if (value <= Constants.QualityControlExpired)
            {
                QcBarIconVisible = true;
                QcBarIcon = "wo_quality_control_red.png";
                QualityControlViewModel.ImagePath = "quality_control_red.png";
                QualityControlViewModel.ValueColor = Color.Red;
                QualityControlViewModel.Description = "Mode Status is \"Failed\" or \"Expired\"";
            }
            else if (value <= Constants.QualityControlExpirationWarning)
            {
                QcBarIconVisible = true;
                QcBarIcon = "wo_quality_control_yellow.png";
                QualityControlViewModel.ImagePath = "quality_control_yellow.png";
                QualityControlViewModel.ValueColor = Color.FromHex("#333");
                QualityControlViewModel.Description = "Mode Status is \"Warning\"";
            }
            else
            {
                QcBarIconVisible = false;
                QualityControlViewModel.ImagePath = "quality_control_green.png";
                QualityControlViewModel.ValueColor = Color.FromHex("#333");
                QualityControlViewModel.Description = "Mode Status is \"Pass\"";
            }
        }

        public void UpdateBluetooth()
        {
            if (BluetoothConnected)
            {
                BluetoothBarIcon = "wo_bluetooth_green.png";
                BluetoothViewModel.ImagePath = "bluetooth_green.png";
                BluetoothViewModel.ValueColor = Color.Black;
                BluetoothViewModel.Label = "Connected";
                BluetoothViewModel.Value = string.Empty;
                BluetoothViewModel.ButtonText = "Settings";
                BluetoothViewModel.Description = "Device is connected.";
            }
            else
            {
                BluetoothBarIcon = "wo_bluetooth_red.png";
                BluetoothViewModel.ImagePath = "bluetooth_red.png";
                BluetoothViewModel.ValueColor = Color.Red;
                BluetoothViewModel.Label = "Disconnected";
                BluetoothViewModel.Value = string.Empty;
                BluetoothViewModel.ButtonText = string.Empty;
                BluetoothViewModel.Description = "No device in range. Device not connected.";
            }
        }

        public void UpdatePressure()
        {
            if (!BluetoothConnected)
            {
                PressureBarIconVisible = false;
                PressureViewModel.ImagePath = "pressure_red.png";
                PressureViewModel.Label = string.Empty;
                PressureViewModel.Value = string.Empty;
                PressureViewModel.ButtonText = string.Empty;
                PressureViewModel.Description = string.Empty;
                return;
            }

            if (Services.DeviceService.Current != null && Services.DeviceService.Current.EnvironmentalInfo.Pressure != 0)
            {
                double value = Services.DeviceService.Current.EnvironmentalInfo.Pressure;

                PressureViewModel.Value = $"{value.ToString("N1", CultureInfo.CurrentCulture)} kPa";
                PressureViewModel.ButtonText = "Info";

                if (value < Constants.PressureLow75)
                {
                    PressureBarIconVisible = true;
                    PressureBarIcon = "wo_pressure_red.png";
                    PressureViewModel.ImagePath = "pressure_red.png";
                    PressureViewModel.ValueColor = Color.Red;
                    PressureViewModel.Label = "Out of Range";
                    PressureViewModel.Description = "The ambient pressure is too low. FeNO testing is disabled until the pressure is higher.";
                }
                else if (value < Constants.PressureWarning78)
                {
                    PressureBarIconVisible = true;
                    PressureBarIcon = "wo_pressure_yellow.png";
                    HumidityViewModel.ImagePath = "pressure_yellow.png";
                    PressureViewModel.ValueColor = Color.FromHex("#333");
                    PressureViewModel.Label = "Warning Range";
                    PressureViewModel.Description = "The ambient pressure is near low limit.";
                }
                else if (value > Constants.PressureHigh110)
                {
                    PressureBarIconVisible = true;
                    PressureBarIcon = "wo_pressure_red.png";
                    PressureViewModel.ImagePath = "pressure_red.png";
                    PressureViewModel.ValueColor = Color.Red;
                    PressureViewModel.Label = "Out of Range";
                    PressureViewModel.Description = "The ambient pressure is too high. FeNO testing is disabled until the pressure is lower.";
                }
                else if (value > Constants.PressureWarning108)
                {
                    PressureBarIconVisible = true;
                    PressureBarIcon = "wo_pressure_yellow.png";
                    HumidityViewModel.ImagePath = "pressure_yellow.png";
                    PressureViewModel.ValueColor = Color.FromHex("#333");
                    PressureViewModel.Label = "Warning Range";
                    PressureViewModel.Description = "The ambient pressure is near high limit.";
                }
                else
                {
                    PressureBarIconVisible = false;
                    PressureViewModel.ImagePath = "pressure_green.png";
                    PressureViewModel.ValueColor = Color.Green;
                    PressureViewModel.Label = "Within Range";
                    PressureViewModel.Description = "The ambient pressure is within operating range.";
                }
            }

            
        }

        public void UpdateTemperature()
        {
            if (!BluetoothConnected)
            {
                TemperatureBarIconVisible = false;

                TemperatureViewModel.ImagePath = "temperature_red.png";
                TemperatureViewModel.Label = string.Empty;
                TemperatureViewModel.Value = string.Empty;
                TemperatureViewModel.ButtonText = string.Empty;
                TemperatureViewModel.Description = string.Empty;
                return;
            }

            if (Services.DeviceService.Current != null)
            {
                double value = Services.DeviceService.Current.EnvironmentalInfo.Temperature;

                TemperatureViewModel.Value = $"{value.ToString("N1", CultureInfo.CurrentCulture)} °C";
                TemperatureViewModel.ButtonText = "Info";

                if (value < Constants.TemperatureLow14)
                {
                    TemperatureBarIconVisible = true;
                    TemperatureBarIcon = "wo_temperature_red.png";
                    TemperatureViewModel.ImagePath = "temperature_red.png";
                    TemperatureViewModel.ValueColor = Color.Red;
                    TemperatureViewModel.Label = "Out of Range";
                    TemperatureViewModel.Description = "The device is too cold. Move the device to a warmer location. FeNO testing is disabled until it has warmed up.";
                }
                else if (value > Constants.TemperatureHigh35)
                {
                    TemperatureBarIconVisible = true;
                    TemperatureBarIcon = "wo_temperature_red.png";
                    TemperatureViewModel.ImagePath = "temperature_red.png";
                    TemperatureViewModel.ValueColor = Color.FromHex("#333");
                    TemperatureViewModel.Label = "Out of Range";
                    TemperatureViewModel.Description = "The device is too warm. Move the device to a cooler location. FeNO testing is disabled until it has cooled down.";
                }
                else
                {
                    TemperatureBarIconVisible = false;
                    TemperatureViewModel.ImagePath = "temperature_green.png";
                    TemperatureViewModel.ValueColor = Color.FromHex("#333");
                    TemperatureViewModel.Label = "Within Range";
                    TemperatureViewModel.Description = "The device temperature is within operating range.";
                }
            }            
        }

        public void UpdateHumidity()
        {
            if (!BluetoothConnected)
            {
                HumidityBarIconVisible = false;

                HumidityViewModel.ImagePath = "humidity_red.png";
                HumidityViewModel.Label = string.Empty;
                HumidityViewModel.Value = string.Empty;
                HumidityViewModel.ButtonText = string.Empty;
                HumidityViewModel.Description = string.Empty;
                return;
            }

            if (Services.DeviceService.Current != null && Services.DeviceService.Current.EnvironmentalInfo.Humidity != 0)
            {
                double value = Services.DeviceService.Current.EnvironmentalInfo.Humidity;

                HumidityViewModel.Value = $"{value.ToString("N1", CultureInfo.CurrentCulture)}%";
                HumidityViewModel.ButtonText = "Info";

                if (value < Constants.HumidityLow18)
                {
                    HumidityBarIconVisible = true;
                    HumidityBarIcon = "wo_humidity_red.png";
                    HumidityViewModel.ImagePath = "humidity_red.png";
                    HumidityViewModel.ValueColor = Color.Red;
                    HumidityViewModel.Label = "Out of Range";
                    HumidityViewModel.Description = "The ambient humidity is too low. Move the device to a more humid location. FeNO testing is disabled until the humidity is higher.";
                }
                else if (value < Constants.HumidityWarning25)
                {
                    HumidityBarIconVisible = true;
                    HumidityBarIcon = "wo_humidity_yellow.png";
                    HumidityViewModel.ImagePath = "humidity_yellow.png";
                    HumidityViewModel.ValueColor = Color.FromHex("#333");
                    HumidityViewModel.Label = "Warning Range";
                    HumidityViewModel.Description = "The ambient humidity is low. Move the device to a more humid location.";
                }
                else if (value > Constants.HumidityHigh92)
                {
                    HumidityBarIconVisible = true;
                    HumidityBarIcon = "wo_humidity_red.png";
                    HumidityViewModel.ImagePath = "humidity_red.png";
                    HumidityViewModel.ValueColor = Color.Red;
                    HumidityViewModel.Label = "Out of Range";
                    HumidityViewModel.Description = "The ambient humidity is too high. Move the device to a drier location. FeNO testing is disabled until the humidity is lower.";
                }
                else if (value > Constants.HumidityWarning85)
                {
                    HumidityBarIconVisible = true;
                    HumidityBarIcon = "wo_humidity_yellow.png";
                    HumidityViewModel.ImagePath = "humidity_yellow.png";
                    HumidityViewModel.ValueColor = Color.FromHex("#333");
                    HumidityViewModel.Label = "Warning Range";
                    HumidityViewModel.Description = "The ambient humidity is high. Move the device to a less humid location.";
                }
                else
                {
                    HumidityBarIconVisible = false;
                    HumidityViewModel.ImagePath = "humidity_green.png";
                    HumidityViewModel.ValueColor = Color.FromHex("#333");
                    HumidityViewModel.Label = "Within Range";
                    HumidityViewModel.Description = "The ambient humidity is within operating range.";
                }
            }            
        }

        public void UpdateBattery()
        {
            if (!BluetoothConnected || Services.DeviceService.Current == null)
            {
                BatteryBarIconVisible = false;
                BatteryViewModel.ImagePath = "battery_red.png";
                BatteryViewModel.Label = string.Empty;
                BatteryViewModel.Value = string.Empty;
                BatteryViewModel.ButtonText = string.Empty;
                BatteryViewModel.Description = string.Empty;
                return;
            }
            if (Services.DeviceService.Current != null)
            {
                float value = Services.DeviceService.Current.EnvironmentalInfo.BatteryLevel;
                BatteryViewModel.Value = $"{value}%";
                BatteryViewModel.ButtonText = "Info";

                BatteryBarIconVisible = true; // Always visible when device is connected

                // 0x4a -- not charging
                // 0x4b -- charging
                // 0x00 -- unknown
                bool batteryCharging = (Services.DeviceService.Current?.DeviceStatusInfo.StatusCode == 0x4b);

                if (batteryCharging)
                {
                    // ToDo; Need to finish implementation
                    if (value > Constants.BatteryWarning20)
                    {
                        BatteryBarIcon = "wo_battery_charge_green.png";
                        BatteryViewModel.ImagePath = "battery_charge_green.png";
                        BatteryViewModel.ValueColor = Color.FromHex("#333");
                        BatteryViewModel.Label = "Charge";
                        BatteryViewModel.Description = "Battery charge OK.";
                    }
                    else if (value >= Constants.BatteryCritical3)
                    {
                        BatteryBarIcon = "wo_battery_charge_yellow.png";
                        BatteryViewModel.ImagePath = "battery_charge_yellow.png";
                        BatteryViewModel.ValueColor = Color.FromHex("#333");
                        BatteryViewModel.Label = "Warning";
                        BatteryViewModel.Description = "Battery charge is low. Now Charging.";
                    }
                    else
                    {
                        BatteryBarIcon = "wo_battery_charge_red.png";
                        BatteryViewModel.ImagePath = "battery_charge_red.png";
                        BatteryViewModel.ValueColor = (System.Drawing.Color)Color.Red;
                        BatteryViewModel.Label = "Low";
                        BatteryViewModel.Description = "Battery charge is critically low. Charging.";
                    }
                }
                else
                {
                    if (value > Constants.BatteryLevel75)
                    {
                        BatteryBarIcon = "wo_battery_green_100.png";
                        BatteryViewModel.ImagePath = "battery_green_100.png";
                        BatteryViewModel.ValueColor = Color.FromHex("#333");
                        BatteryViewModel.Label = "Charge";
                        BatteryViewModel.Description = "Battery charge OK.";
                    }
                    else if (value > Constants.BatteryLevel50)
                    {
                        BatteryBarIcon = "wo_battery_green_75.png";
                        BatteryViewModel.ImagePath = "battery_green_75.png";
                        BatteryViewModel.ValueColor = Color.FromHex("#333");
                        BatteryViewModel.Label = "Charge";
                        BatteryViewModel.Description = "Battery charge OK.";
                    }
                    else if (value > Constants.BatteryWarning20)
                    {
                        BatteryBarIcon = "wo_battery_green_50.png";
                        BatteryViewModel.ImagePath = "battery_green_50.png";
                        BatteryViewModel.ValueColor = Color.FromHex("#333");
                        BatteryViewModel.Label = "Charge";
                        BatteryViewModel.Description = "Battery charge OK.";
                    }
                    else if (value >= Constants.BatteryCritical3)
                    {
                        BatteryBarIcon = "wo_battery_charge_yellow.png";
                        BatteryViewModel.ImagePath = "battery_yellow.png";
                        BatteryViewModel.ValueColor = Color.FromHex("#333");
                        BatteryViewModel.Label = "Warning";
                        BatteryViewModel.Description = "Battery charge is low. Connect the device to an outlet with the supplied USB-C cord.";
                    }
                    else
                    {
                        BatteryBarIcon = "wo_battery_charge_red.png";
                        BatteryViewModel.ImagePath = "battery_red.png";
                        BatteryViewModel.ValueColor = Color.Red;
                        BatteryViewModel.Label = "Critical";
                        BatteryViewModel.Description = "Battery charge is critically low. Connect the device to an outlet with the supplied USB-C cord.";
                    }
                }
            }
            
        }

        [RelayCommand]
        private void ShowDeviceDetails()
        {
            if (BluetoothConnected)
            {
                Services.Navigation.ShowStatusDetailsPopup(DeviceViewModel);
            }
            else
            {
                Services.Dialogs.ShowToast("Device not connected", 4);
            }
        }

        [RelayCommand]
        private void ShowSensorDetails()
        {
            if (BluetoothConnected)
            {
                Services.Navigation.ShowStatusDetailsPopup(SensorViewModel);
            }
            else
            {
                Services.Dialogs.ShowToast("Device not connected", 4);
            }
        }

        [RelayCommand]
        private void ShowQualityControlDetails()
        {
            if (BluetoothConnected)
            {
                Services.Navigation.ShowStatusDetailsPopup(QualityControlViewModel);
            }
            else
            {
                Services.Dialogs.ShowToast("Device not connected", 4);
            }
        }

        [RelayCommand]
        private void ShowBluetoothDetails()
        {
            Services.Navigation.ShowStatusDetailsPopup(BluetoothViewModel);
        }

        [RelayCommand]
        private void ShowPressureDetails()
        {
            if (BluetoothConnected)
            {
                Services.Navigation.ShowStatusDetailsPopup(PressureViewModel);
            }
            else
            {
                Services.Dialogs.ShowToast("Device not connected", 4);
            }
        }

        [RelayCommand]
        private void ShowTemperatureDetails()
        {
            if (BluetoothConnected)
            {
                Services.Navigation.ShowStatusDetailsPopup(TemperatureViewModel);
            }
            else
            {
                Services.Dialogs.ShowToast("Device not connected", 4);
            }
        }

        [RelayCommand]
        private void ShowHumidityDetails()
        {
            if (BluetoothConnected)
            {
                Services.Navigation.ShowStatusDetailsPopup(HumidityViewModel);
            }
            else
            {
                Services.Dialogs.ShowToast("Device not connected", 4);
            }
        }

        [RelayCommand]
        private void ShowBatteryDetails()
        {
            if (BluetoothConnected)
            {
                Services.Navigation.ShowStatusDetailsPopup(BatteryViewModel);
            }
            else
            {
                Services.Dialogs.ShowToast("Device not connected", 4);
            }
        }

        [RelayCommand]
        private async Task NavigateToStatusPageAsync()
        {

            switch (App.GetCurrentPage())
            {
                //case TestErrorView _:     // Seems to be OK to navigate away from this
                //case TestFailedView _:   // Seems to be OK to navigate away from this
                case BreathManeuverFeedbackView _:
                case PreparingStandardTestResultView _:
                case StopExhalingView _:
                case TestResultsView _:
                case TestErrorView _:
                case QCNegativeControlTestView _:
                case QCNegativeControlResultView _:
                case QCNegativeControlChartView _:
                case QCUserTestView _:
                case QCUserStopTestView _:
                case QCUserTestCalculationView _:
                case QCUserTestResultView _:
                case QCUserTestErrorView _:
                case QCUserTestChartView _:

                // This view means it still in scanning BLE, tap the bluetooth icon should navigate to nowhere
                case DevicePowerOnView _:  
                    // Do not navigate to DeviceStatusHubView when on the pages (breath test in progress)
                    break;
                case DashboardView _:
                    if (BluetoothConnected) { goto default; }
                    else break; 
                default:
                    await Services.Navigation.DeviceStatusHubView();
                    break;
            }

        }

    }
}
