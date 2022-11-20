using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Timers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FenomPlus.Controls;
using FenomPlus.Interfaces;
using FenomPlus.Models;
using FenomPlus.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Svg;
using static FenomPlus.SDK.Core.FenomHubSystemDiscovery;

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

        private readonly Timer DeviceStatusTimer = new Timer(30000);


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

            SetDefaults();

            RefreshIconStatus();

            DeviceStatusTimer.Elapsed += DeviceStatusTimerOnElapsed;
            DeviceStatusTimer.Start();

            // Received whenever a Bluetooth connect or disconnect occurs - Bluetooth not updated through timer
            WeakReferenceMessenger.Default.Register<DeviceConnectedMessage>(this, (r, m) =>
            {
                UpdateBluetooth(Services.BleHub.IsConnected());
            });
        }

        private void SetDefaults()
        {
            SerialNumber = string.Empty;
            FirmwareVersion = string.Empty;

            BluetoothBarIcon = "wo_bluetooth_red.png";
            BluetoothViewModel.ImagePath = "bluetooth_red.png";
            BluetoothViewModel.Color = Color.Red;
            BluetoothViewModel.Label = "Disconnected";
            BluetoothViewModel.Value = "";

            //SensorViewModel.Header = "Sensor";
            SensorBarIcon = "wo_sensor_red.png";
            SensorViewModel.ImagePath = "sensor_red.png";
            SensorViewModel.Color = Color.Red;
            SensorViewModel.Label = "Disconnected";
            SensorViewModel.Value = "";

            //DeviceViewModel.Header = "Device";
            DeviceBarIcon = "wo_device_red.png";
            DeviceViewModel.ImagePath = "device_red.png";
            DeviceViewModel.Color = Color.Red;
            DeviceViewModel.Label = "Disconnected";
            DeviceViewModel.Value = "";

            //QualityControlViewModel.Header = "Quality Control";
            QcBarIcon = "wo_quality_control_red.png";
            QualityControlViewModel.ImagePath = "quality_control_red.png";
            QualityControlViewModel.Color = Color.Red;
            QualityControlViewModel.Label = "Disconnected";
            QualityControlViewModel.Value = "";

            //HumidityViewModel.Header = "Humidity";
            HumidityBarIcon = "wo_humidity_red.png";
            HumidityViewModel.ImagePath = "humidity_red.png";
            HumidityViewModel.Color = Color.Red;
            HumidityViewModel.Label = "Disconnected";
            HumidityViewModel.Value = "";

            //PressureViewModel.Header = "Pressure";
            PressureBarIcon = "wo_pressure_red.png";
            PressureViewModel.ImagePath = "pressure_red.png";
            PressureViewModel.Color = Color.Red;
            PressureViewModel.Label = "Disconnected";
            PressureViewModel.Value = "";

            //TemperatureViewModel.Header = "Temperature";
            TemperatureBarIcon = "wo_temperature_red.png";
            TemperatureViewModel.ImagePath = "temperature_red.png";
            TemperatureViewModel.Color = Color.Red;
            TemperatureViewModel.Label = "Disconnected";
            TemperatureViewModel.Value = "";

            //BatteryViewModel.Header = "Battery";
            BatteryBarIcon = "wo_battery_red.png";
            BatteryViewModel.ImagePath = "battery_red.png";
            BatteryViewModel.Color = Color.Red;
            BatteryViewModel.Label = "Disconnected";
            BatteryViewModel.Value = "";



        }

        [RelayCommand]
        private void NavigateToStatusPage()
        {
            Services.Navigation.DeviceStatusView();
        }

        private void DeviceStatusTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            // Get latest environmental info
            Services.BleHub.RequestEnvironmentalInfo();

            RefreshIconStatus();
        }

        public void RefreshIconStatus()
        {
            if (Services.BleHub.IsConnected())
            {
                SerialNumber = $"Device Serial Number ({Services.Cache.DeviceSerialNumber})";
                FirmwareVersion = $"Firmware ({Services.Cache.Firmware})";
            }
            else
            {
                SerialNumber = string.Empty;
                FirmwareVersion = string.Empty;
                SetDefaults();
            }


            //UpdateBluetooth(Services.BleHub.IsConnected()); // Cache is updated when characteristic changes

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

        [ObservableProperty]
        private bool _bluetoothConnected;

        public void UpdateBluetooth(bool connected)
        {
            BluetoothConnected = connected;

            BluetoothViewModel.ButtonText = "Settings";

            if (BluetoothConnected)
            {
                BluetoothBarIcon = "wo_bluetooth_green.png";
                BluetoothViewModel.ImagePath = "bluetooth_green.png";
                BluetoothViewModel.Color = Color.Green;
                BluetoothViewModel.Label = "Connected";
                BluetoothViewModel.Value = "OK";
            }
            else
            {
                BluetoothBarIcon = "wo_bluetooth_red.png";
                BluetoothViewModel.ImagePath = "bluetooth_red.png";
                BluetoothViewModel.Color = Color.Red;
                BluetoothViewModel.Label = "Disconnected";
                BluetoothViewModel.Value = "";
            }
        }

        public const int BatteryCritical = 3;
        public const int BatteryWarning = 20;
        public const int Battery50 = 50;
        public const int Battery75 = 75;
        public const int Battery100 = 100;

        public void UpdateBattery(int value)
        {
            BatteryViewModel.Value = $"{value}%";
            BatteryViewModel.ButtonText = "Order";

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
                // full
                SensorBarIcon = "wo_sensor_green";
                SensorViewModel.ImagePath = "sensor_green";
                SensorViewModel.Color = Color.Green;
            }
        }

        public const int QualityControlExpirationLow = 0;
        public const int QualityControlExpirationWarning = 1;
        public const int QualityControlExpirationFull = 7;

        public void UpdateQualityControlExpiration(int value)
        {
            QualityControlViewModel.Value = $"{(int)((value < 365) ? value : value / 365)}";
            QualityControlViewModel.Label = "Days Left";
            QualityControlViewModel.ButtonText = "Settings";

            if (value <= QualityControlExpirationLow)
            {
                // low
                QcBarIcon = "wo_quality_control_red.png";
                QualityControlViewModel.ImagePath = "quality_control_red.png";
                QualityControlViewModel.Color = Color.Red;
            }
            else if (value <= QualityControlExpirationWarning)
            {
                // warning
                QcBarIcon = "wo_quality_control_yellow.png";
                QualityControlViewModel.ImagePath = "quality_control_yellow.png";
                QualityControlViewModel.Color = Color.Yellow;
            }
            else
            {
                // full
                QcBarIcon = "wo_quality_control_green.png";
                QualityControlViewModel.ImagePath = "quality_control_green.png";
                QualityControlViewModel.Color = Color.Green;
            }
        }

        public const int DeviceLow = 0;
        public const int DeviceWarning = 60;
        public const int DeviceFull = 1 * 365 + 10;

        public void UpdateDevice(int value)
        {
            DeviceViewModel.Value = $"{value}";
            DeviceViewModel.ButtonText = "Order";
            DeviceViewModel.Label = "Days Left";

            if (value <= DeviceLow)
            {
                // low
                DeviceBarIcon = "wo_device_red.png";
                DeviceViewModel.ImagePath = "device_red.png";
                DeviceViewModel.Color = Color.Red;
            }
            else if (value <= DeviceWarning)
            {
                // warning
                DeviceBarIcon = "_3x_wo_device_yellow.png";
                DeviceViewModel.ImagePath = "_3x_device_yellow.png";
                DeviceViewModel.Color = Color.Yellow;
            }
            else
            {
                // full
                DeviceBarIcon = "wo_device_green_100.png";
                DeviceViewModel.ImagePath = "device_green_100.png";
                DeviceViewModel.Color = Color.Green;
            }
        }

        public const int RelativeHumidityLow = 0;
        public const int RelativeHumidityWarning = 80;
        public const int RelativeHumidityFull = 100;

        public void UpdateRelativeHumidity(int value)
        {
            HumidityViewModel.Value = $"{value}%";
            HumidityViewModel.ButtonText = "Info";

            if (value <= RelativeHumidityLow)
            {
                // low
                HumidityBarIcon = "wo_humidity_red.png";
                HumidityViewModel.ImagePath = "humidity_red.png";
                HumidityViewModel.Color = Color.Red;
                HumidityViewModel.Label = "Out of Range";
            }
            else if (value <= RelativeHumidityWarning)
            {
                // warning
                HumidityBarIcon = "wo_humidity_yellow.png";
                HumidityViewModel.ImagePath = "humidity_yellow.png";
                HumidityViewModel.Color = Color.Yellow;
                HumidityViewModel.Label = "Warning Range";
            }
            else
            {
                // full
                HumidityBarIcon = "wo_humidity_green.png";
                HumidityViewModel.ImagePath = "humidity_green.png";
                HumidityViewModel.Color = Color.Green;
                HumidityViewModel.Label = "In Range";
            }
        }

        public const int TemperatureLow = 0;
        public const int TemperatureWarning = 80;
        public const int TemperatureFull = 100;

        public void UpdateTemperature(int value)
        {
            TemperatureViewModel.Value = $"{value} °C";
            TemperatureViewModel.ButtonText = "Info";

            if (value <= TemperatureLow)
            {
                // low
                TemperatureBarIcon = "wo_temperature_red.png";
                TemperatureViewModel.ImagePath = "temperature_red.png";
                TemperatureViewModel.Color = Color.Red;
                TemperatureViewModel.Label = "Out of Range";
            }
            else if (value <= TemperatureWarning)
            {
                // warning
                TemperatureBarIcon = "wo_temperature_yellow.png";
                TemperatureViewModel.ImagePath = "temperature_yellow.png";
                TemperatureViewModel.Color = Color.Yellow;
                TemperatureViewModel.Label = "Warning Range";
            }
            else
            {
                // full
                TemperatureBarIcon = "wo_temperature_green.png";
                TemperatureViewModel.ImagePath = "temperature_green.png";
                TemperatureViewModel.Color = Color.Green;
                TemperatureViewModel.Label = "In Range";
            }
        }

        public const int PressureLow = 0;
        public const int PressureWarning = 80;
        public const int PressureFull = 100;

        public void UpdatePressure(int value)
        {
            PressureViewModel.Value = $"{value} kPa";
            PressureViewModel.ButtonText = "Info";

            if (value <= PressureLow)
            {
                // low
                PressureBarIcon = "wo_pressure_red.png";
                PressureViewModel.ImagePath = "pressure_red.png";
                PressureViewModel.Color = Color.Red;
                PressureViewModel.Label = "Out of Range";
            }
            else if (value <= PressureWarning)
            {
                // warning
                PressureBarIcon = "wo_pressure_yellow.png";
                PressureViewModel.ImagePath = "pressure_yellow.png";
                PressureViewModel.Color = Color.Yellow;
                PressureViewModel.Label = "Warning Range";
            }
            else
            {
                // full
                PressureBarIcon = "wo_pressure_green.png";
                PressureViewModel.ImagePath = "pressure_green.png";
                PressureViewModel.Color = Color.Green;
                PressureViewModel.Label = "In Range";
            }
        }

        [RelayCommand]
        private void ShowSensorDetails()
        {
            Services.Navigation.ShowStatusDetailsPopup(SensorViewModel);
        }

        [RelayCommand]
        private void ShowDeviceDetails()
        {
            Services.Navigation.ShowStatusDetailsPopup(DeviceViewModel);
        }

        [RelayCommand]
        private void ShowQualityControlDetails()
        {
            Services.Navigation.ShowStatusDetailsPopup(QualityControlViewModel);
        }

        [RelayCommand]
        private void ShowBluetoothDetails()
        {
            Services.Navigation.ShowStatusDetailsPopup(BluetoothViewModel);
        }

        [RelayCommand]
        private void ShowHumidityDetails()
        {
            Services.Navigation.ShowStatusDetailsPopup(HumidityViewModel);
        }

        [RelayCommand]
        private void ShowPressureDetails()
        {
            Services.Navigation.ShowStatusDetailsPopup(PressureViewModel);
        }

        [RelayCommand]
        private void ShowTemperatureDetails()
        {
            Services.Navigation.ShowStatusDetailsPopup(TemperatureViewModel);
        }

        [RelayCommand]
        private void ShowBatteryDetails()
        {
            Services.Navigation.ShowStatusDetailsPopup(BatteryViewModel);
        }



    }
}
