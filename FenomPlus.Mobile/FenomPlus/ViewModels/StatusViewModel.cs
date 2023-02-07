 using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FenomPlus.Controls;
using FenomPlus.Views;
using System;
using System.Diagnostics;
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
        private const int RequestNewStatusInterval = 15;        // New DeviceInfo and EnvironmentInfo every 15 seconds

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
            _ = RefreshStatusAsync();

            BluetoothStatusTimer = new Timer(TimerIntervalMilliseconds);
            BluetoothStatusTimer.Elapsed += BluetoothCheck;
            BluetoothStatusTimer.Start();
        }

        private bool CheckDeviceConnection()
        {
            if (Services == null || Services.DeviceService == null || Services.DeviceService.Current == null)
                return false;

            bool deviceIsConnected = Services.DeviceService.Current.Connected;

            // Don't use Services.BleHub.IsConnected() or it will try to reconnect - we just want current connection status
            return deviceIsConnected;
        }

        private int BluetoothCheckCount = 0;

        private async void BluetoothCheck(object sender, ElapsedEventArgs e)
        {
            // Note:  All device status parameters are conditional on the bluetooth connection

            BluetoothConnected = CheckDeviceConnection();
            //Debug.WriteLine($"BluetoothCheck: {BluetoothConnected}");

            if (BluetoothConnected)
            {
                if (App.GetCurrentPage() is DevicePowerOnView)  // ToDo: Only needed because viewmodels never die
                {
                    // Only navigate if during startup
                    await Services.Navigation.DashboardView();
                }

                if (BluetoothCheckCount == 0)
                {
                    if (Services is { DeviceService: { Current: { } } })
                    {
                        //await Services.DeviceService.Current.RequestDeviceInfo();
                        //await Services.DeviceService.Current.RequestEnvironmentalInfo();
                    }

                    Debug.WriteLine("UpdateDeviceAndEnvironmentalInfoAsync");
                }

                BluetoothCheckCount++;

                if (BluetoothCheckCount >= RequestNewStatusInterval)
                    BluetoothCheckCount = 0;
            }
            else
            {
                BluetoothCheckCount = 0; // Reset counter
            }

            //Debug.WriteLine($"BluetoothCheckCount: {BluetoothCheckCount}");

            await RefreshStatusAsync();
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

            RefreshInProgress = true;

            UpdateVersionNumbers();

            UpdateBluetooth();

            UpdateDevice(Services.Cache.DeviceExpireDate);

            if (Services.DeviceService.Current != null)
            {
                UpdateSensor(Services.DeviceService.Current.SensorExpireDate);

                UpdateBattery(Services.DeviceService.Current.EnvironmentalInfo.BatteryLevel);
                // Don't update  an environment value if it is zero, we must not have good value yet

                //if (Services.Cache.EnvironmentalInfo.BatteryLevel != 0)
                //    UpdateBattery(Services.Cache.EnvironmentalInfo.BatteryLevel); // Cache is updated when characteristic changes

                if (Services.DeviceService.Current.EnvironmentalInfo.Pressure != 0)
                    UpdatePressure(Services.DeviceService.Current.EnvironmentalInfo.Pressure);

                if (Services.DeviceService.Current.EnvironmentalInfo.Humidity != 0)
                    UpdateHumidity(Services.DeviceService.Current.EnvironmentalInfo.Humidity);

                if (Services.DeviceService.Current.EnvironmentalInfo.Temperature != 0)
                    UpdateTemperature(Services.DeviceService.Current.EnvironmentalInfo.Temperature);
            }

            

            UpdateQualityControlExpiration(7); // ToDo:  Need value here

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

            // ToDo: Remove hard coded value
            expirationDate = DateTime.Now.AddYears(5).AddDays(-10);
            //expirationDate = DateTime.Now.AddDays(5); 

            int daysRemaining = (expirationDate > DateTime.Now) ? (int)(expirationDate - DateTime.Now).TotalDays : 0;

            DeviceViewModel.ButtonText = "Order";

            if (daysRemaining <= Constants.DeviceExpired)
            {
                DeviceBarIconVisible = true;
                DeviceBarIcon = "wo_device_red.png";
                DeviceViewModel.ImagePath = "device_red.png";
                DeviceViewModel.ValueColor = Color.Red;
                DeviceViewModel.Description = "The device has reached the end of its lifespan. Contact Customer Service for a replacement."; DeviceViewModel.Value = $"{daysRemaining}";
                DeviceViewModel.Label = "Days Left";
            }
            else if (daysRemaining <= Constants.DeviceWarning60Days)
            {
                DeviceBarIconVisible = true;
                DeviceBarIcon = "_3x_wo_device_yellow.png";
                DeviceViewModel.ImagePath = "device_yellow.png";
                DeviceViewModel.ValueColor = Color.FromHex("#333");
                DeviceViewModel.Description = "The device will expire in less than 60 days. Contact Customer Service.";
                DeviceViewModel.Value = $"{daysRemaining}";
                DeviceViewModel.Label = "Days Left";
            }
            else
            {
                DeviceBarIconVisible = false;
                DeviceViewModel.ImagePath = "device_green_100.png";
                DeviceViewModel.ValueColor = Color.FromHex("#333");
                DeviceViewModel.Description = $"The device has {daysRemaining} days remaining before it has to be replaced.";
                DeviceViewModel.Value = $"{daysRemaining / 30}";
                DeviceViewModel.Label = "Months Left";
            }
        }

        public void UpdateSensor(DateTime expirationDate)
        {
            if (!BluetoothConnected)
            {
                SensorBarIconVisible = false;

                SensorViewModel.ImagePath = "sensor_red.png";
                SensorViewModel.Label = string.Empty;
                SensorViewModel.Value = string.Empty;
                SensorViewModel.ButtonText = string.Empty;
                SensorViewModel.Description = string.Empty;
                return;
            }

            // ToDo: Remove hard coded value
            expirationDate = DateTime.Now.AddMonths(18).AddDays(-10);
            //expirationDate = DateTime.Now.AddDays(5); 

            int daysRemaining = (expirationDate > DateTime.Now) ? (int)(expirationDate - DateTime.Now).TotalDays : 0;

            SensorBarIconVisible = true;
            SensorViewModel.ButtonText = "Order";

            if (daysRemaining <= Constants.SensorExpired)
            {
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

        public void UpdatePressure(double value)
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

        public void UpdateTemperature(double value)
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

        public void UpdateHumidity(double value)
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

        public void UpdateBattery(int value)
        {
            if (!BluetoothConnected)
            {
                BatteryBarIconVisible = false;
                BatteryViewModel.ImagePath = "battery_red.png";
                BatteryViewModel.Label = string.Empty;
                BatteryViewModel.Value = string.Empty;
                BatteryViewModel.ButtonText = string.Empty;
                BatteryViewModel.Description = string.Empty;
                return;
            }

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
                else if (value > Constants.BatteryCritical3)
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
                    BatteryViewModel.ValueColor = (System.Drawing.Color) Color.Red;
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
                else if (value > Constants.BatteryCritical3)
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
            await RefreshStatusAsync();

            switch (App.GetCurrentPage())
            {
                //case TestErrorView testErrorView:     // Seems to be OK to navigate away from this
                //case TestFailedView testFailedView:   // Seems to be OK to navigate away from this
                case BreathManeuverFeedbackView breathManeuverFeedbackView:
                case PreparingStandardTestResultView preparingStandardTestResultView:
                case StopExhalingView stopExhalingView:
                case TestResultsView testResultsView:
                case QCNegativeControlTestView qCNegativeControlTestView:
                case QCNegativeControlResultView qCNegativeControlResultView:
                case QCUserTestView qCUserTestView:
                case QCUserStopTestView qCUserStopTestView:
                case QCUserTestResultView qCUserTestResultView:
                    // Do not navigate to DeviceStatusHubView when on the pages (breath test in progress)
                    break;
                default:
                    await Services.Navigation.DeviceStatusHubView();
                    break;
            }

        }

    }
}
