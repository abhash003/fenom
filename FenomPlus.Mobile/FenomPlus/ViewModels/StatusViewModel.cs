using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
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
using FenomPlus.Views;
using Plugin.BLE.Abstractions.Contracts;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Svg;
using static FenomPlus.SDK.Core.FenomHubSystemDiscovery;

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

            //Received whenever a Bluetooth connect or disconnect occurs -Problem code because this message lags the actual event by seconds
            //WeakReferenceMessenger.Default.Register<DeviceConnectedMessage>(this, (r, m) =>
            //{
            //    //BluetoothStatusTimer.Stop(); // Stop the timer so we don't crash with it

            //    //// Force an update

            //    //BluetoothCheck(null, null);

            //    //BluetoothCheckCount = 10; // Start on advanced count to get more instantaneous feedback
            //    //BluetoothStatusTimer.Start();
            //});
        }

        private bool CheckDeviceConnection()
        {
            if (Services == null || Services.BleHub == null || Services.BleHub.BleDevice == null)
                return false;

            bool deviceIsConnected = Services.BleHub.BleDevice.Connected;

            if (App.GetCurrentPage() is DevicePowerOnView && deviceIsConnected)  // ToDo: Only needed because viewmodels never die
            {
                // Only navigate if during startup
                Services.Navigation.DashboardView();
            }

            // Don't use Services.BleHub.IsConnected() or it will try to reconnect - we just want current connection status
            return deviceIsConnected;
        }

        private int BluetoothCheckCount = 0;

        private async void BluetoothCheck(object sender, ElapsedEventArgs e)
        {
            // Note:  All device status parameters are conditional on the bluetooth connection

            BluetoothConnected = CheckDeviceConnection();
            Debug.WriteLine($"BluetoothCheck: {BluetoothConnected}");

            if (BluetoothConnected)
            {

                if (BluetoothCheckCount == 0)
                {
                    await Services.BleHub.RequestDeviceInfo();
                    await Services.BleHub.RequestEnvironmentalInfo();

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

            Debug.WriteLine($"BluetoothCheckCount: {BluetoothCheckCount}");

            await RefreshStatusAsync();
        }

        private bool RefreshInProgress = false;

        public async Task RefreshStatusAsync()
        {
            if (RefreshInProgress) // Prevents a collison of requests
            {
                await Task.Delay(1);
                return;
            }

            if (BluetoothConnected && Services.BleHub.BreathTestInProgress)
            {
                // Don't update environmental properties during test
                return;
            }

            // To early to get status
            if (Cache == null || Cache.EnvironmentalInfo == null)
                return;

            RefreshInProgress = true;

            UpdateVersionNumbers();

            UpdateBluetooth();

            UpdateDevice(Cache.DeviceExpireDate);

            UpdateSensor(Cache.SensorExpireDate);

            UpdateBattery(Cache.EnvironmentalInfo.BatteryLevel); // Cache is updated when characteristic changes

            UpdatePressure(Cache.EnvironmentalInfo.Pressure);

            UpdateHumidity(Cache.EnvironmentalInfo.Humidity);

            UpdateTemperature(Cache.EnvironmentalInfo.Temperature);

            UpdateQualityControlExpiration(7); // ToDo:  Need value here

            RefreshInProgress = false;
        }

        public void UpdateVersionNumbers()
        {
            if (BluetoothConnected)
            {
                DeviceSerialNumber = $"Device Serial Number ({Services.Cache.DeviceSerialNumber})";
                DeviceFirmwareVersion = $"Firmware ({Services.Cache.Firmware})";
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

            if (value <= Constants.PressureLow75)
            {
                PressureBarIconVisible = true;
                PressureBarIcon = "wo_pressure_red.png";
                PressureViewModel.ImagePath = "pressure_red.png";
                PressureViewModel.ValueColor = Color.Red;
                PressureViewModel.Label = "Out of Range";
                PressureViewModel.Description = "The ambient pressure is too low. FeNO testing is disabled until the pressure is higher.";
            }
            else if (value >= Constants.PressureHigh110)
            {
                PressureBarIconVisible = true;
                PressureBarIcon = "wo_pressure_red.png";
                PressureViewModel.ImagePath = "pressure_red.png";
                PressureViewModel.ValueColor = Color.Red;
                PressureViewModel.Label = "Out of Range";
                PressureViewModel.Description = "The ambient pressure is too high. FeNO testing is disabled until the pressure is lower.";
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

            if (value <= Constants.TemperatureLow15)
            {
                TemperatureBarIconVisible = true;
                TemperatureBarIcon = "wo_temperature_red.png";
                TemperatureViewModel.ImagePath = "temperature_red.png";
                TemperatureViewModel.ValueColor = Color.Red;
                TemperatureViewModel.Label = "Out of Range";
                TemperatureViewModel.Description = "The device is too cold. Move the device to a warmer location. FeNO testing is disabled until it has warmed up.";
            }
            else if (value >= Constants.TemperatureHigh35)
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

            if (value <= Constants.HumidityLow18)
            {
                HumidityBarIconVisible = true;
                HumidityBarIcon = "wo_humidity_red.png";
                HumidityViewModel.ImagePath = "humidity_red.png";
                HumidityViewModel.ValueColor = Color.Red;
                HumidityViewModel.Label = "Out of Range";
                HumidityViewModel.Description = "The ambient humidity is too low. Move the device to a more humid location. FeNO testing is disabled until the humidity is higher.";
            }
            else if (value <= Constants.HumidityWarning25)
            {
                HumidityBarIconVisible = true; 
                HumidityBarIcon = "wo_humidity_yellow.png";
                HumidityViewModel.ImagePath = "humidity_yellow.png";
                HumidityViewModel.ValueColor = Color.FromHex("#333");
                HumidityViewModel.Label = "Warning Range";
                HumidityViewModel.Description = "The ambient humidity is low. Move the device to a more humid location.";
            }
            else if (value >= Constants.HumidityHigh90)
            {
                HumidityBarIconVisible = true;
                HumidityBarIcon = "wo_humidity_red.png";
                HumidityViewModel.ImagePath = "humidity_red.png";
                HumidityViewModel.ValueColor = Color.Red;
                HumidityViewModel.Label = "Out of Range";
                HumidityViewModel.Description = "The ambient humidity is too high. Move the device to a drier location. FeNO testing is disabled until the humidity is lower.";
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

            bool batteryCharging = false; // ToDo: Hard coded for now

            if (batteryCharging)
            {
                // ToDo; Need to finish implementation
                if (value > Constants.BatteryWarning20)
                {
                    BatteryBarIcon = "wo_battery_charge_green_50.png";
                    BatteryViewModel.ImagePath = "battery_charge_green_50.png";
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
                    BatteryViewModel.ValueColor = Color.Red;
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
                    BatteryViewModel.ImagePath = "battery__red.png";
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
            await Services.Navigation.DeviceStatusView();
        }

    }
}
