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
        public StatusInfoBoxViewModel SensorInfoViewModel = new StatusInfoBoxViewModel();
        public StatusInfoBoxViewModel DeviceInfoViewModel = new StatusInfoBoxViewModel();
        public StatusInfoBoxViewModel QualityControlInfoViewModel = new StatusInfoBoxViewModel();
        public StatusInfoBoxViewModel BluetoothInfoViewModel = new StatusInfoBoxViewModel();
        public StatusInfoBoxViewModel HumidityInfoViewModel = new StatusInfoBoxViewModel();
        public StatusInfoBoxViewModel PressureInfoViewModel = new StatusInfoBoxViewModel();
        public StatusInfoBoxViewModel TemperatureInfoViewModel = new StatusInfoBoxViewModel();
        public StatusInfoBoxViewModel BatteryInfoViewModel = new StatusInfoBoxViewModel();

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

            BluetoothInfoViewModel.Header = "Bluetooth";
            SensorInfoViewModel.Header = "Sensor";
            DeviceInfoViewModel.Header = "Device";
            QualityControlInfoViewModel.Header = "Quality Control";
            HumidityInfoViewModel.Header = "Humidity";
            PressureInfoViewModel.Header = "Pressure";
            TemperatureInfoViewModel.Header = "Temperature";
            BatteryInfoViewModel.Header = "Battery";

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
            BluetoothInfoViewModel.ImagePath = "bluetooth_red.png";
            BluetoothInfoViewModel.Color = Color.Red;
            BluetoothInfoViewModel.Label = "Disconnected";
            BluetoothInfoViewModel.Value = "";

            //SensorInfoViewModel.Header = "Sensor";
            SensorBarIcon = "wo_sensor_red.png";
            SensorInfoViewModel.ImagePath = "sensor_red.png";
            SensorInfoViewModel.Color = Color.Red;
            SensorInfoViewModel.Label = "Disconnected";
            SensorInfoViewModel.Value = "";

            //DeviceInfoViewModel.Header = "Device";
            DeviceBarIcon = "wo_device_red.png";
            DeviceInfoViewModel.ImagePath = "device_red.png";
            DeviceInfoViewModel.Color = Color.Red;
            DeviceInfoViewModel.Label = "Disconnected";
            DeviceInfoViewModel.Value = "";

            //QualityControlInfoViewModel.Header = "Quality Control";
            QcBarIcon = "wo_quality_control_red.png";
            QualityControlInfoViewModel.ImagePath = "quality_control_red.png";
            QualityControlInfoViewModel.Color = Color.Red;
            QualityControlInfoViewModel.Label = "Disconnected";
            QualityControlInfoViewModel.Value = "";

            //HumidityInfoViewModel.Header = "Humidity";
            HumidityBarIcon = "wo_humidity_red.png";
            HumidityInfoViewModel.ImagePath = "humidity_red.png";
            HumidityInfoViewModel.Color = Color.Red;
            HumidityInfoViewModel.Label = "Disconnected";
            HumidityInfoViewModel.Value = "";

            //PressureInfoViewModel.Header = "Pressure";
            PressureBarIcon = "wo_pressure_red.png";
            PressureInfoViewModel.ImagePath = "pressure_red.png";
            PressureInfoViewModel.Color = Color.Red;
            PressureInfoViewModel.Label = "Disconnected";
            PressureInfoViewModel.Value = "";

            //TemperatureInfoViewModel.Header = "Temperature";
            TemperatureBarIcon = "wo_temperature_red.png";
            TemperatureInfoViewModel.ImagePath = "temperature_red.png";
            TemperatureInfoViewModel.Color = Color.Red;
            TemperatureInfoViewModel.Label = "Disconnected";
            TemperatureInfoViewModel.Value = "";

            //BatteryInfoViewModel.Header = "Battery";
            BatteryBarIcon = "wo_battery_red.png";
            BatteryInfoViewModel.ImagePath = "battery_red.png";
            BatteryInfoViewModel.Color = Color.Red;
            BatteryInfoViewModel.Label = "Disconnected";
            BatteryInfoViewModel.Value = "";



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

            BluetoothInfoViewModel.ButtonText = "Settings";

            if (BluetoothConnected)
            {
                BluetoothBarIcon = "wo_bluetooth_green.png";
                BluetoothInfoViewModel.ImagePath = "bluetooth_green.png";
                BluetoothInfoViewModel.Color = Color.Green;
                BluetoothInfoViewModel.Label = "Connected";
                BluetoothInfoViewModel.Value = "OK";
            }
            else
            {
                BluetoothBarIcon = "wo_bluetooth_red.png";
                BluetoothInfoViewModel.ImagePath = "bluetooth_red.png";
                BluetoothInfoViewModel.Color = Color.Red;
                BluetoothInfoViewModel.Label = "Disconnected";
                BluetoothInfoViewModel.Value = "";
            }
        }

        public const int BatteryCritical = 3;
        public const int BatteryWarning = 20;
        public const int Battery50 = 50;
        public const int Battery75 = 75;
        public const int Battery100 = 100;

        public void UpdateBattery(int value)
        {
            BatteryInfoViewModel.Value = $"{value}%";
            BatteryInfoViewModel.ButtonText = "Order";

            if (value > Battery75)
            {
                BatteryBarIcon = "wo_battery_green_100.png";
                BatteryInfoViewModel.ImagePath = "battery_green_100.png";
                BatteryInfoViewModel.Color = Color.Green;
                BatteryInfoViewModel.Label = "Charge";
            }
            else if (value > Battery50)
            {
                BatteryBarIcon = "wo_battery_green_75.png";
                BatteryInfoViewModel.ImagePath = "battery_green_75.png";
                BatteryInfoViewModel.Color = Color.Green;
                BatteryInfoViewModel.Label = "Charge";
            }
            else if (value > BatteryWarning)
            {
                BatteryBarIcon = "wo_battery_green_50.png";
                BatteryInfoViewModel.ImagePath = "battery_green_50.png";
                BatteryInfoViewModel.Color = Color.Green;
                BatteryInfoViewModel.Label = "Charge";
            }
            else if (value > BatteryCritical)
            {
                BatteryBarIcon = "wo_battery_charge_yellow.png";
                BatteryInfoViewModel.ImagePath = "battery_charge_yellow.png";
                BatteryInfoViewModel.Color = Color.Yellow;
                BatteryInfoViewModel.Label = "Warning";
            }
            else
            {
                BatteryBarIcon = "wo_battery_charge_red.png";
                BatteryInfoViewModel.ImagePath = "battery_charge_red.png";
                BatteryInfoViewModel.Color = Color.Red;
                BatteryInfoViewModel.Label = "Low";
            }
        }

        public const int SensorLow = 0;
        public const int SensorWarning = 60;

        public void UpdateSensor(int value)
        {
            SensorInfoViewModel.Value = $"{(int)((value < 365) ? value : value / 365)}";
            SensorInfoViewModel.Label = "Days Left";
            SensorInfoViewModel.ButtonText = "Order";


            if (value <= SensorLow)
            {
                // low
                SensorBarIcon = "wo_sensor_red.png";
                SensorInfoViewModel.ImagePath = "sensor_red.png";
                SensorInfoViewModel.Color = Color.Red;
            }
            else if (value <= SensorWarning)
            {
                // warning
                SensorBarIcon = "wo_sensor_yellow.png";
                SensorInfoViewModel.ImagePath = "sensor_yellow.png";
                SensorInfoViewModel.Color = Color.Yellow;
            }
            else
            {
                // full
                SensorBarIcon = "wo_sensor_green";
                SensorInfoViewModel.ImagePath = "sensor_green";
                SensorInfoViewModel.Color = Color.Green;
            }
        }

        public const int QualityControlExpirationLow = 0;
        public const int QualityControlExpirationWarning = 1;
        public const int QualityControlExpirationFull = 7;

        public void UpdateQualityControlExpiration(int value)
        {
            QualityControlInfoViewModel.Value = $"{(int)((value < 365) ? value : value / 365)}";
            QualityControlInfoViewModel.Label = "Days Left";
            QualityControlInfoViewModel.ButtonText = "Settings";

            if (value <= QualityControlExpirationLow)
            {
                // low
                QcBarIcon = "wo_quality_control_red.png";
                QualityControlInfoViewModel.ImagePath = "quality_control_red.png";
                QualityControlInfoViewModel.Color = Color.Red;
            }
            else if (value <= QualityControlExpirationWarning)
            {
                // warning
                QcBarIcon = "wo_quality_control_yellow.png";
                QualityControlInfoViewModel.ImagePath = "quality_control_yellow.png";
                QualityControlInfoViewModel.Color = Color.Yellow;
            }
            else
            {
                // full
                QcBarIcon = "wo_quality_control_green.png";
                QualityControlInfoViewModel.ImagePath = "quality_control_green.png";
                QualityControlInfoViewModel.Color = Color.Green;
            }
        }

        public const int DeviceLow = 0;
        public const int DeviceWarning = 60;
        public const int DeviceFull = 1 * 365 + 10;

        public void UpdateDevice(int value)
        {
            DeviceInfoViewModel.Value = $"{value}";
            DeviceInfoViewModel.ButtonText = "Order";
            DeviceInfoViewModel.Label = "Days Left";

            if (value <= DeviceLow)
            {
                // low
                DeviceBarIcon = "wo_device_red.png";
                DeviceInfoViewModel.ImagePath = "device_red.png";
                DeviceInfoViewModel.Color = Color.Red;
            }
            else if (value <= DeviceWarning)
            {
                // warning
                DeviceBarIcon = "_3x_wo_device_yellow.png";
                DeviceInfoViewModel.ImagePath = "_3x_device_yellow.png";
                DeviceInfoViewModel.Color = Color.Yellow;
            }
            else
            {
                // full
                DeviceBarIcon = "wo_device_green_100.png";
                DeviceInfoViewModel.ImagePath = "device_green_100.png";
                DeviceInfoViewModel.Color = Color.Green;
            }
        }

        public const int RelativeHumidityLow = 0;
        public const int RelativeHumidityWarning = 80;
        public const int RelativeHumidityFull = 100;

        public void UpdateRelativeHumidity(int value)
        {
            HumidityInfoViewModel.Value = $"{value}%";
            HumidityInfoViewModel.ButtonText = "Info";

            if (value <= RelativeHumidityLow)
            {
                // low
                HumidityBarIcon = "wo_humidity_red.png";
                HumidityInfoViewModel.ImagePath = "humidity_red.png";
                HumidityInfoViewModel.Color = Color.Red;
                HumidityInfoViewModel.Label = "Out of Range";
            }
            else if (value <= RelativeHumidityWarning)
            {
                // warning
                HumidityBarIcon = "wo_humidity_yellow.png";
                HumidityInfoViewModel.ImagePath = "humidity_yellow.png";
                HumidityInfoViewModel.Color = Color.Yellow;
                HumidityInfoViewModel.Label = "Warning Range";
            }
            else
            {
                // full
                HumidityBarIcon = "wo_humidity_green.png";
                HumidityInfoViewModel.ImagePath = "humidity_green.png";
                HumidityInfoViewModel.Color = Color.Green;
                HumidityInfoViewModel.Label = "In Range";
            }
        }

        public const int TemperatureLow = 0;
        public const int TemperatureWarning = 80;
        public const int TemperatureFull = 100;

        public void UpdateTemperature(int value)
        {
            TemperatureInfoViewModel.Value = $"{value} °C";
            TemperatureInfoViewModel.ButtonText = "Info";

            if (value <= TemperatureLow)
            {
                // low
                TemperatureBarIcon = "wo_temperature_red.png";
                TemperatureInfoViewModel.ImagePath = "temperature_red.png";
                TemperatureInfoViewModel.Color = Color.Red;
                TemperatureInfoViewModel.Label = "Out of Range";
            }
            else if (value <= TemperatureWarning)
            {
                // warning
                TemperatureBarIcon = "wo_temperature_yellow.png";
                TemperatureInfoViewModel.ImagePath = "temperature_yellow.png";
                TemperatureInfoViewModel.Color = Color.Yellow;
                TemperatureInfoViewModel.Label = "Warning Range";
            }
            else
            {
                // full
                TemperatureBarIcon = "wo_temperature_green.png";
                TemperatureInfoViewModel.ImagePath = "temperature_green.png";
                TemperatureInfoViewModel.Color = Color.Green;
                TemperatureInfoViewModel.Label = "In Range";
            }
        }

        public const int PressureLow = 0;
        public const int PressureWarning = 80;
        public const int PressureFull = 100;

        public void UpdatePressure(int value)
        {
            PressureInfoViewModel.Value = $"{value} kPa";
            PressureInfoViewModel.ButtonText = "Info";

            if (value <= PressureLow)
            {
                // low
                PressureBarIcon = "wo_pressure_red.png";
                PressureInfoViewModel.ImagePath = "pressure_red.png";
                PressureInfoViewModel.Color = Color.Red;
                PressureInfoViewModel.Label = "Out of Range";
            }
            else if (value <= PressureWarning)
            {
                // warning
                PressureBarIcon = "wo_pressure_yellow.png";
                PressureInfoViewModel.ImagePath = "pressure_yellow.png";
                PressureInfoViewModel.Color = Color.Yellow;
                PressureInfoViewModel.Label = "Warning Range";
            }
            else
            {
                // full
                PressureBarIcon = "wo_pressure_green.png";
                PressureInfoViewModel.ImagePath = "pressure_green.png";
                PressureInfoViewModel.Color = Color.Green;
                PressureInfoViewModel.Label = "In Range";
            }
        }

        [ObservableProperty]
        private bool _showDetail;

        [ObservableProperty] 
        private string _detailsHeader;

        [ObservableProperty]
        private string _detailsImagePath;

        [ObservableProperty]
        private string _detailsValue;

        [ObservableProperty]
        private string _detailsLabel;

        [ObservableProperty]
        private string _detailsDescription;

        [RelayCommand]
        private void ShowSensorDetails()
        {
            //DetailsHeader = SensorInfoViewModel.Header;
            //DetailsImagePath = SensorInfoViewModel.ImagePath;
            //DetailsValue = SensorInfoViewModel.Value;
            //DetailsLabel = SensorInfoViewModel.Label;
            //DetailsDescription = SensorInfoViewModel.Description;
            //ShowDetail = true;

            Services.Navigation.ShowStatusDetailsPopup(SensorInfoViewModel);
        }

        [RelayCommand]
        private void ShowDeviceDetails()
        {
            DetailsHeader = DeviceInfoViewModel.Header;
            DetailsImagePath = DeviceInfoViewModel.ImagePath;
            DetailsValue = DeviceInfoViewModel.Value;
            DetailsLabel = DeviceInfoViewModel.Label;
            DetailsDescription = DeviceInfoViewModel.Description;
            ShowDetail = true;
        }

        [RelayCommand]
        private void ShowQualityControlDetails()
        {
            DetailsHeader = QualityControlInfoViewModel.Header;
            DetailsImagePath = QualityControlInfoViewModel.ImagePath;
            DetailsValue = QualityControlInfoViewModel.Value;
            DetailsLabel = QualityControlInfoViewModel.Label;
            DetailsDescription = QualityControlInfoViewModel.Description;
            ShowDetail = true;
        }

        [RelayCommand]
        private void ShowBluetoothDetails()
        {
            DetailsHeader = BluetoothInfoViewModel.Header;
            DetailsImagePath = BluetoothInfoViewModel.ImagePath;
            DetailsValue = BluetoothInfoViewModel.Value;
            DetailsLabel = BluetoothInfoViewModel.Label;
            DetailsDescription = BluetoothInfoViewModel.Description;
            ShowDetail = true;
        }

        [RelayCommand]
        private void ShowHumidityDetails()
        {
            DetailsHeader = HumidityInfoViewModel.Header;
            DetailsImagePath = HumidityInfoViewModel.ImagePath;
            DetailsValue = HumidityInfoViewModel.Value;
            DetailsLabel = HumidityInfoViewModel.Label;
            DetailsDescription = HumidityInfoViewModel.Description;
            ShowDetail = true;
        }

        [RelayCommand]
        private void ShowPressureDetails()
        {
            DetailsHeader = PressureInfoViewModel.Header;
            DetailsImagePath = PressureInfoViewModel.ImagePath;
            DetailsValue = PressureInfoViewModel.Value;
            DetailsLabel = PressureInfoViewModel.Label;
            DetailsDescription = PressureInfoViewModel.Description;
            ShowDetail = true;
        }

        [RelayCommand]
        private void ShowTemperatureDetails()
        {
            DetailsHeader = TemperatureInfoViewModel.Header;
            DetailsImagePath = TemperatureInfoViewModel.ImagePath;
            DetailsValue = TemperatureInfoViewModel.Value;
            DetailsLabel = TemperatureInfoViewModel.Label;
            DetailsDescription = TemperatureInfoViewModel.Description;
            ShowDetail = true;
        }

        [RelayCommand]
        private void ShowBatteryDetails()
        {
            DetailsHeader = BatteryInfoViewModel.Header;
            DetailsImagePath = BatteryInfoViewModel.ImagePath;
            DetailsValue = BatteryInfoViewModel.Value;
            DetailsLabel = BatteryInfoViewModel.Label;
            DetailsDescription = BatteryInfoViewModel.Description;
            ShowDetail = true;
        }



    }
}
