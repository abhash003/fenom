using System.ComponentModel.Design;
using FenomPlus.SDK.Core.Ble.PluginBLE;
using FenomPlus.Services;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Xamarin.Essentials;
using Xamarin.Forms.Svg;
using Xamarin.Forms;

namespace FenomPlus.ViewModels
{
    public partial class StatusDeviceInfoViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string _softwareVersion;

        [ObservableProperty]
        private string _serialNumber;

        [ObservableProperty]
        private string _firmwareVersion;

        // Sensor Block

        [ObservableProperty]
        private string _sensorTitle = "Sensor";
        [ObservableProperty]
        public ImageSource _sensorIconSource = SvgImageSource.FromSvgResource("_battery_red.svg", 32, 32);
        [ObservableProperty]
        public string _sensorValue = "0";
        [ObservableProperty]
        public string _sensorLabel = "Days left";
        [ObservableProperty]
        public string _sensorButtonText = "Order";
        [RelayCommand]
        public void SensorButton()
        {

        }

        // Device Block
        [ObservableProperty]
        public string _deviceTitle = "Device";
        [ObservableProperty]
        public string _deviceIconSource;
        [ObservableProperty]
        public string _deviceValue = "0";
        [ObservableProperty]
        public string _deviceLabel = "Days left";
        [ObservableProperty]
        public string _deviceButtonText = "Order";
        [RelayCommand]
        public void DeviceButton()
        {

        }

        // Quality Control Block
        [ObservableProperty]
        public string _qcTitle = "Quality Control";
        [ObservableProperty]
        public string _qcIconSource;
        [ObservableProperty]
        public string _qcValue = "0";
        [ObservableProperty]
        public string _qcLabel = "Days left";
        [ObservableProperty]
        public string _qcButtonText= "Settings";
        [RelayCommand]
        public void QualityControl()
        {

        }

        // Bluetooth Block
        [ObservableProperty]
        public string _bluetoothTitle = "Bluetooth";
        [ObservableProperty]
        public string _bluetoothIconSource = "BluetoothIcon.png";
        [ObservableProperty]
        public string _bluetoothValue = string.Empty;
        [ObservableProperty]
        public string _bluetoothLabel = "Disconnected";
        [ObservableProperty]
        public string _bluetoothButtonText = "Settings";
        [RelayCommand]
        public void Bluetooth()
        {

        }

        // Humidity Block
        [ObservableProperty]
        public string _humidityTitle = "Humidity";
        [ObservableProperty]
        public string _humidityIconSource;

        [ObservableProperty] 
        public string _humidityValue = "18%";
        [ObservableProperty]
        public string _humidityLabel = "Out of Range";
        [ObservableProperty]
        public string _humidityButtonText = "Info";
        [RelayCommand]
        public void Humidity()
        {

        }

        // Pressure Block
        public string _pressureTitle = "Pressure";
        [ObservableProperty]
        public string _pressureIconSource;
        [ObservableProperty]
        public string _pressureValue = "110.65 kPa";
        [ObservableProperty]
        public string _pressureLabel = "Out of Range";
        [ObservableProperty]
        public string _pressureButtonText = "Info";
        [RelayCommand]
        public void Pressure()
        {

        }

        // Temperature Block
        public string _temperatureTitle = "Temperature";
        [ObservableProperty]
        public string _temperatureIconSource;
        [ObservableProperty]
        public string _temperatureValue = "14.6 &#176;C";
        [ObservableProperty]
        public string _temperatureLabel = "Out of Range";
        [ObservableProperty]
        public string _temperatureButtonText = "Info";
        [RelayCommand]
        public void Temperature()
        {

        }

        // Battery Block
        public string _batteryTitle = "Battery";
        [ObservableProperty]
        public string _batteryIconSource;
        [ObservableProperty]
        public string _batteryValue = "5%";
        [ObservableProperty]
        public string _batteryLabel = "Charge";
        [ObservableProperty]
        public string _batteryButtonText = "Info";
        [RelayCommand]
        public void BatteryCommand()
        {

        }

        public bool DeviceIsConnected => Services.BleHub.IsConnected();

        public StatusDeviceInfoViewModel()
        {
            VersionTracking.Track();
            SoftwareVersion = $"Serial Number {VersionTracking.CurrentVersion}";

            var device = Services.BleHub.BleDevice;

            if (device.Connected)
            {
                DeviceSerialNumber = $"Serial Number {Services.Cache.DeviceSerialNumber}";
                FirmwareVersion = $"Firmware {Services.Cache.Firmware}";

                BluetoothLabel = "Connected";
            }
            else
            {
                BluetoothLabel = "Disconnected";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnAppearing()
        {
            base.OnAppearing();
            //Services.BleHub.IsNotConnectedRedirect();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void NewGlobalData()
        {
            base.NewGlobalData();
        }
    }
}
