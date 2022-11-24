using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FenomPlus.Controls;
using FenomPlus.Interfaces;
using FenomPlus.Models;
using FenomPlus.Services;
using FenomPlus.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Svg;
using static FenomPlus.SDK.Core.FenomHubSystemDiscovery;

// Note: Shared by DeviceHubView and TitleContentView

namespace FenomPlus.ViewModels
{
    public partial class StatusViewModel : BaseViewModel
    {
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

        [ObservableProperty]
        private string _softwareVersion;

        [ObservableProperty]
        private string _serialNumber;

        [ObservableProperty]
        private string _firmwareVersion;

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

        private readonly Timer BluetoothStatusTimer = new Timer(1000);

        public StatusViewModel()
        {
            VersionTracking.Track();

            SoftwareVersion = $"Software ({VersionTracking.CurrentVersion})";

            BluetoothViewModel.Header = "Bluetooth";
            SensorViewModel.Header = "Sensor";
            DeviceViewModel.Header = "Device";
            QualityControlViewModel.Header = "Quality Control";
            HumidityViewModel.Header = "Humidity";
            PressureViewModel.Header = "Pressure";
            TemperatureViewModel.Header = "Temperature";
            BatteryViewModel.Header = "Battery";

            BluetoothConnected = false;
            RefreshStatus();

            BluetoothStatusTimer.Elapsed += BluetoothCheck;
            BluetoothStatusTimer.Start();

            // Received whenever a Bluetooth connect or disconnect occurs - Bluetooth not updated through timer
            WeakReferenceMessenger.Default.Register<DeviceConnectedMessage>(this, (r, m) =>
            {
                Debug.WriteLine($"Device is connected = {m.Value}");
                BluetoothStatusTimer.Stop();

                // Force an update

                BluetoothCheck(null, null);

                BluetoothCheckCount = 10; // Start on advanced count to get more instantaneous feedback
                BluetoothStatusTimer.Start();
            });
        }

        //private void SetDisconnectedDefaults()
        //{
        //    SerialNumber = string.Empty;
        //    FirmwareVersion = string.Empty;

        //    BatteryBarIconVisible = false;

        //    SensorBarIcon = "wo_sensor_red.png";
        //    SensorBarIconVisible = false;

        //    SensorViewModel.ImagePath = "sensor_red.png";
        //    SensorViewModel.Color = Color.Red;
        //    SensorViewModel.Label = string.Empty;
        //    SensorViewModel.Value = string.Empty;

        //    DeviceBarIcon = "wo_device_red.png";
        //    DeviceBarIconVisible = false;

        //    DeviceViewModel.ImagePath = "device_red.png";
        //    DeviceViewModel.Color = Color.Red;
        //    DeviceViewModel.Label = string.Empty;
        //    DeviceViewModel.Value = string.Empty;

        //    QcBarIcon = "wo_quality_control_red.png";
        //    QcBarIconVisible = false;

        //    QualityControlViewModel.ImagePath = "quality_control_red.png";
        //    QualityControlViewModel.Color = Color.Red;
        //    QualityControlViewModel.Label = string.Empty;
        //    QualityControlViewModel.Value = string.Empty;

        //    HumidityBarIcon = "wo_humidity_red.png";
        //    HumidityBarIconVisible = false;

        //    HumidityViewModel.ImagePath = "humidity_red.png";
        //    HumidityViewModel.Color = Color.Red;
        //    HumidityViewModel.Label = string.Empty;
        //    HumidityViewModel.Value = string.Empty;

        //    PressureBarIcon = "wo_pressure_red.png";
        //    PressureBarIconVisible = false;

        //    PressureViewModel.ImagePath = "pressure_red.png";
        //    PressureViewModel.Color = Color.Red;
        //    PressureViewModel.Label = string.Empty;
        //    PressureViewModel.Value = string.Empty;

        //    TemperatureBarIcon = "wo_temperature_red.png";
        //    TemperatureBarIconVisible = false;

        //    TemperatureViewModel.ImagePath = "temperature_red.png";
        //    TemperatureViewModel.Color = Color.Red;
        //    TemperatureViewModel.Label = string.Empty;
        //    TemperatureViewModel.Value = string.Empty;

        //    BatteryBarIcon = "wo_battery_red.png";

        //    BatteryViewModel.ImagePath = "battery_red.png";
        //    BatteryViewModel.Color = Color.Red;
        //    BatteryViewModel.Label = string.Empty;
        //    BatteryViewModel.Value = string.Empty;

        //    BluetoothBarIcon = "wo_bluetooth_red.png";
        //    BluetoothViewModel.ImagePath = "bluetooth_red.png";
        //    BluetoothViewModel.Color = Color.Red;
        //    BluetoothViewModel.Label = "Disconnected";
        //    BluetoothViewModel.Value = string.Empty;
        //}

        private int BluetoothCheckCount = 0;

        private bool CheckDeviceConnection()
        {
            // More robust way to determine if bluetooth device is connected
            return Services is { BleHub: { BleDevice: { Connected: true } } };
        }

        private async void BluetoothCheck(object sender, ElapsedEventArgs e)
        {
            // Note:  All device status parameters are conditional on the bluetooth connection

            BluetoothCheckCount++;
            Debug.WriteLine($"Counter = {BluetoothCheckCount}");

            BluetoothConnected = CheckDeviceConnection();

            if (BluetoothCheckCount == 10)
            {
                if (BluetoothConnected)
                {
                    // Get latest environmental info
                    await Services.BleHub.RequestEnvironmentalInfo();
                }

                BluetoothCheckCount = 0; // Reset
            }

            Device.BeginInvokeOnMainThread(RefreshStatus);
        }

        public void RefreshStatus()
        {
            if (BluetoothConnected && Services.BleHub.BreathTestInProgress)
            {
                // Don't update environmental properties during test
                return;
            }

            // To early to get status
            if (Cache == null || Cache.EnvironmentalInfo == null)
                return;

            UpdateVersionNumbers();

            UpdateBluetooth();

            UpdateBattery(Cache.EnvironmentalInfo.BatteryLevel); // Cache is updated when characteristic changes

            int daysRemaining = (Cache.SensorExpireDate > DateTime.Now) ? (int)(Cache.SensorExpireDate - DateTime.Now).TotalDays : 0;
            UpdateDevice(daysRemaining);

            UpdateSensor((Cache.SensorExpireDate > DateTime.Now) ? (int)(Cache.SensorExpireDate - DateTime.Now).TotalDays : 0);

            UpdateQualityControlExpiration(0); // ToDo:  Need value here

            UpdatePressure(Cache.EnvironmentalInfo.Pressure);

            UpdateRelativeHumidity(Cache.EnvironmentalInfo.Humidity);

            UpdateTemperature(Cache.EnvironmentalInfo.Temperature);

            Debug.WriteLine("Note: Status icons updated!");
        }

        public void UpdateVersionNumbers()
        {
            if (BluetoothConnected)
            {
                SerialNumber = $"Device Serial Number ({Services.Cache.DeviceSerialNumber})";
                FirmwareVersion = $"Firmware ({Services.Cache.Firmware})";
            }
            else
            {
                SerialNumber = string.Empty;
                FirmwareVersion = string.Empty;
            }
        }

        public void UpdateBluetooth()
        {
            if (BluetoothConnected)
            {
                BluetoothBarIcon = "wo_bluetooth_green.png";
                BluetoothViewModel.ImagePath = "bluetooth_green.png";
                BluetoothViewModel.Color = Color.Green;
                BluetoothViewModel.Label = "Connected";
                BluetoothViewModel.Value = string.Empty;
                BluetoothViewModel.ButtonText = "Settings";
            }
            else
            {
                BluetoothBarIcon = "wo_bluetooth_red.png";
                BluetoothViewModel.ImagePath = "bluetooth_red.png";
                BluetoothViewModel.Color = Color.Red;
                BluetoothViewModel.Label = "Disconnected";
                BluetoothViewModel.Value = string.Empty;
                BluetoothViewModel.ButtonText = string.Empty;
            }
        }

        public const int BatteryCritical = 3;
        public const int BatteryWarning = 20;
        public const int Battery50 = 50;
        public const int Battery75 = 75;
        public const int Battery100 = 100;

        public void UpdateBattery(int value)
        {
            if (!BluetoothConnected)
            {
                BatteryBarIconVisible = false;

                BatteryViewModel.ImagePath = "battery_red.png";
                BatteryViewModel.Color = Color.Red;
                BatteryViewModel.Label = string.Empty;
                BatteryViewModel.Value = string.Empty;
                BatteryViewModel.ButtonText = string.Empty;
                return;
            }

            BatteryViewModel.Value = $"{value}%";
            BatteryViewModel.ButtonText = "Order";

            BatteryBarIconVisible = true; // Always visible when device is connected

            if (value > Battery75)
            {
                BatteryBarIcon = "wo_battery_green_100.png";
                BatteryViewModel.ImagePath = "battery_green_100.png";
                BatteryViewModel.Color = Color.Green;
                BatteryViewModel.Label = "Charge";
            }
            else if (value > Battery50)
            {
                BatteryBarIcon = "wo_battery_green_75.png";
                BatteryViewModel.ImagePath = "battery_green_75.png";
                BatteryViewModel.Color = Color.Green;
                BatteryViewModel.Label = "Charge";
            }
            else if (value > BatteryWarning)
            {
                BatteryBarIcon = "wo_battery_green_50.png";
                BatteryViewModel.ImagePath = "battery_green_50.png";
                BatteryViewModel.Color = Color.Green;
                BatteryViewModel.Label = "Charge";
            }
            else if (value > BatteryCritical)
            {
                BatteryBarIcon = "wo_battery_charge_yellow.png";
                BatteryViewModel.ImagePath = "battery_charge_yellow.png";
                BatteryViewModel.Color = Color.Yellow;
                BatteryViewModel.Label = "Warning";
            }
            else
            {
                BatteryBarIcon = "wo_battery_charge_red.png";
                BatteryViewModel.ImagePath = "battery_charge_red.png";
                BatteryViewModel.Color = Color.Red;
                BatteryViewModel.Label = "Low";
            }
        }

        public const int SensorLow = 0;
        public const int SensorWarning = 60;

        public void UpdateSensor(int value)
        {
            if (!BluetoothConnected)
            {
                SensorBarIconVisible = false;

                SensorViewModel.ImagePath = "sensor_red.png";
                SensorViewModel.Color = Color.Red;
                SensorViewModel.Label = string.Empty;
                SensorViewModel.Value = string.Empty;
                SensorViewModel.ButtonText = string.Empty;
                return;
            }

            SensorBarIconVisible = true;
            SensorViewModel.Value = $"{(int)((value < 365) ? value : value / 365)}";
            SensorViewModel.Label = "Days Left";
            SensorViewModel.ButtonText = "Order";

            if (value <= SensorLow)
            {
                // low
                SensorBarIcon = "wo_sensor_red.png";
                SensorViewModel.ImagePath = "sensor_red.png";
                SensorViewModel.Color = Color.Red;
            }
            else if (value <= SensorWarning)
            {
                // warning
                SensorBarIcon = "wo_sensor_yellow.png";
                SensorViewModel.ImagePath = "sensor_yellow.png";
                SensorViewModel.Color = Color.Yellow;
            }
            else
            {
                SensorBarIconVisible = false;
            }
        }

        public const int QualityControlExpirationLow = 0;
        public const int QualityControlExpirationWarning = 1;
        public const int QualityControlExpirationFull = 7;

        public void UpdateQualityControlExpiration(int value)
        {
            if (!BluetoothConnected)
            {
                QcBarIconVisible = false;

                QualityControlViewModel.ImagePath = "quality_control_red.png";
                QualityControlViewModel.Color = Color.Red;
                QualityControlViewModel.Label = string.Empty;
                QualityControlViewModel.Value = string.Empty;
                QualityControlViewModel.ButtonText = string.Empty;
                return;
            }

            QualityControlViewModel.Value = $"{(int)((value < 365) ? value : value / 365)}";
            QualityControlViewModel.Label = "Days Left";
            QualityControlViewModel.ButtonText = "Settings";

            if (value <= QualityControlExpirationLow)
            {
                QcBarIconVisible = true;
                QcBarIcon = "wo_quality_control_red.png";
                QualityControlViewModel.ImagePath = "quality_control_red.png";
                QualityControlViewModel.Color = Color.Red;
            }
            else if (value <= QualityControlExpirationWarning)
            {
                QcBarIconVisible = true;
                QcBarIcon = "wo_quality_control_yellow.png";
                QualityControlViewModel.ImagePath = "quality_control_yellow.png";
                QualityControlViewModel.Color = Color.Yellow;
            }
            else
            {
                QcBarIconVisible = false;
            }
        }

        public const int DeviceLow = 0;
        public const int DeviceWarning = 60;
        public const int DeviceFull = 1 * 365 + 10;

        public void UpdateDevice(int value)
        {
            if (!BluetoothConnected)
            {
                DeviceBarIconVisible = false;

                DeviceViewModel.ImagePath = "device_red.png";
                DeviceViewModel.Color = Color.Red;
                DeviceViewModel.Label = string.Empty;
                DeviceViewModel.Value = string.Empty;
                DeviceViewModel.ButtonText = string.Empty;
                return;
            }

            DeviceViewModel.Value = $"{value}";
            DeviceViewModel.ButtonText = "Order";
            DeviceViewModel.Label = "Days Left";

            if (value <= DeviceLow)
            {
                DeviceBarIconVisible = true;
                DeviceBarIcon = "wo_device_red.png";
                DeviceViewModel.ImagePath = "device_red.png";
                DeviceViewModel.Color = Color.Red;
            }
            else if (value <= DeviceWarning)
            {
                DeviceBarIconVisible = true;
                DeviceBarIcon = "_3x_wo_device_yellow.png";
                DeviceViewModel.ImagePath = "_3x_device_yellow.png";
                DeviceViewModel.Color = Color.Yellow;
            }
            else
            {
                DeviceBarIconVisible = false;
            }
        }

        public const int RelativeHumidityLow = 0;
        public const int RelativeHumidityWarning = 80;
        public const int RelativeHumidityFull = 100;

        public void UpdateRelativeHumidity(int value)
        {
            if (!BluetoothConnected)
            {
                HumidityBarIconVisible = false;

                HumidityViewModel.ImagePath = "humidity_red.png";
                HumidityViewModel.Color = Color.Red;
                HumidityViewModel.Label = string.Empty;
                HumidityViewModel.Value = string.Empty;
                HumidityViewModel.ButtonText = string.Empty;
                return;
            }

            HumidityViewModel.Value = $"{value}%";
            HumidityViewModel.ButtonText = "Info";

            if (value <= RelativeHumidityLow)
            {
                HumidityBarIconVisible = true;
                HumidityBarIcon = "wo_humidity_red.png";
                HumidityViewModel.ImagePath = "humidity_red.png";
                HumidityViewModel.Color = Color.Red;
                HumidityViewModel.Label = "Out of Range";
            }
            else if (value <= RelativeHumidityWarning)
            {
                HumidityBarIconVisible = true;
                HumidityBarIcon = "wo_humidity_yellow.png";
                HumidityViewModel.ImagePath = "humidity_yellow.png";
                HumidityViewModel.Color = Color.Yellow;
                HumidityViewModel.Label = "Warning Range";
            }
            else
            {
                HumidityBarIconVisible = false;
            }
        }

        public const int TemperatureLow = 0;
        public const int TemperatureWarning = 80;
        public const int TemperatureFull = 100;

        public void UpdateTemperature(int value)
        {
            if (!BluetoothConnected)
            {
                TemperatureBarIconVisible = false;

                TemperatureViewModel.ImagePath = "temperature_red.png";
                TemperatureViewModel.Color = Color.Red;
                TemperatureViewModel.Label = string.Empty;
                TemperatureViewModel.Value = string.Empty;
                TemperatureViewModel.ButtonText = string.Empty;
                return;
            }

            TemperatureViewModel.Value = $"{value} °C";
            TemperatureViewModel.ButtonText = "Info";

            if (value <= TemperatureLow)
            {
                TemperatureBarIconVisible = true;
                TemperatureBarIcon = "wo_temperature_red.png";
                TemperatureViewModel.ImagePath = "temperature_red.png";
                TemperatureViewModel.Color = Color.Red;
                TemperatureViewModel.Label = "Out of Range";
            }
            else if (value <= TemperatureWarning)
            {
                TemperatureBarIconVisible = true;
                TemperatureBarIcon = "wo_temperature_yellow.png";
                TemperatureViewModel.ImagePath = "temperature_yellow.png";
                TemperatureViewModel.Color = Color.Yellow;
                TemperatureViewModel.Label = "Warning Range";
            }
            else
            {
                TemperatureBarIconVisible = false;
            }
        }

        public const int PressureLow = 0;
        public const int PressureWarning = 80;
        public const int PressureFull = 100;

        public void UpdatePressure(int value)
        {
            if (!BluetoothConnected)
            {
                PressureBarIconVisible = false;
                PressureViewModel.ImagePath = "pressure_red.png";
                PressureViewModel.Color = Color.Red;
                PressureViewModel.Label = string.Empty;
                PressureViewModel.Value = string.Empty;
                PressureViewModel.ButtonText = string.Empty;
                return;
            }

            PressureViewModel.Value = $"{value} kPa";
            PressureViewModel.ButtonText = "Info";

            if (value <= PressureLow)
            {
                PressureBarIconVisible = true;
                PressureBarIcon = "wo_pressure_red.png";
                PressureViewModel.ImagePath = "pressure_red.png";
                PressureViewModel.Color = Color.Red;
                PressureViewModel.Label = "Out of Range";
            }
            else if (value <= PressureWarning)
            {
                PressureBarIconVisible = true;
                PressureBarIcon = "wo_pressure_yellow.png";
                PressureViewModel.ImagePath = "pressure_yellow.png";
                PressureViewModel.Color = Color.Yellow;
                PressureViewModel.Label = "Warning Range";
            }
            else
            {
                PressureBarIconVisible = false;
            }
        }

        [RelayCommand]
        private void ShowBluetoothDetails()
        {
            Services.Navigation.ShowStatusDetailsPopup(BluetoothViewModel);
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
        private void NavigateToStatusPage()
        {
            Services.Navigation.DeviceStatusView();
        }

    }
}
