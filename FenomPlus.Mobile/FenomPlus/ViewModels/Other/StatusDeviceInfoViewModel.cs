using System.ComponentModel.Design;
using FenomPlus.SDK.Core.Ble.PluginBLE;
using FenomPlus.Services;
using System.Runtime.CompilerServices;
using Xamarin.Essentials;

namespace FenomPlus.ViewModels
{
    public class StatusDeviceInfoViewModel : BaseViewModel
    {
        private string _softwareVersion;
        public string SoftwareVersion
        {
            get => _softwareVersion;
            set
            {
                _softwareVersion = value;
                OnPropertyChanged(nameof(SoftwareVersion));
            }
        }

        private string _softwareBuild;
        public string SoftwareBuild
        {
            get => _softwareBuild;
            set
            {
                _softwareBuild = value;
                OnPropertyChanged(nameof(SoftwareBuild));
            }
        }

        private string _deviceConnectionStatus;
        public string DeviceConnectionStatus
        {
            get => _deviceConnectionStatus;
            set
            {
                _deviceConnectionStatus = value;
                OnPropertyChanged(nameof(DeviceConnectionStatus));
            }
        }

        private string _deviceName;
        public string DeviceName
        {
            get => _deviceName;
            set
            {
                _deviceName = value;
                OnPropertyChanged(nameof(DeviceName));
            }
        }

        private string _manufacturer;
        public string Manufacturer
        {
            get => _manufacturer;
            set
            {
                _manufacturer = value;
                OnPropertyChanged(nameof(Manufacturer));
            }
        }

        private string _firmwareVersion;
        public string FirmwareVersion
        {
            get => _firmwareVersion;
            set
            {
                _firmwareVersion = value;
                OnPropertyChanged(nameof(FirmwareVersion));
            }
        }



        public bool DeviceIsConnected => Services.BleHub.IsConnected();

        public StatusDeviceInfoViewModel()
        {
            VersionTracking.Track();
            SoftwareVersion = VersionTracking.CurrentVersion;
            SoftwareBuild = VersionTracking.CurrentBuild;

            var device = Services.BleHub.BleDevice;

            if (device.Connected)
            {
                DeviceConnectionStatus = "Connected";
                DeviceSerialNumber = Services.Cache.DeviceSerialNumber;
                Firmware = Services.Cache.Firmware;
                DeviceName = device.Name;
                Manufacturer = device.Manufacturer;
                FirmwareVersion = Services.Cache.Firmware;
            }
            else
            {
                DeviceConnectionStatus = "Not Connected";
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
