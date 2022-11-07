using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FenomPlus.Database.Repository.Interfaces;
using FenomPlus.Interfaces;
using FenomPlus.Services;
using System.Threading.Tasks;
using FenomPlus.Models;

namespace FenomPlus.ViewModels
{
    public partial class BaseViewModel : ObservableObject, IBaseServices
    {
        public IAppServices Services => IOC.Services;
        public IBleHubService BleHub => Services.BleHub;
        public ICacheService Cache => Services.Cache;
        public IConfigService Config => Services.Config;
        
        // repos here
        public IBreathManeuverErrorRepository ErrorsRepo => Services.Database.BreathManeuverErrorRepo;
        public IBreathManeuverResultRepository ResultsRepo => Services.Database.BreathManeuverResultRepo;
        public IQualityControlRepository QCRepo => Services.Database.QualityControlRepo;
        public IQualityControlDevicesRepository QCDevicesRepo => Services.Database.QualityControlDevicesRepo;
        public IQualityControlUsersRepository QCUsersRepo => Services.Database.QualityControlUsersRepo;

        [ObservableProperty]
        bool _isBusy = false;

        [ObservableProperty]
        private string _title = string.Empty;

        [ObservableProperty]
        private string _deviceSerialNumber;

        [ObservableProperty]
        private string _firmware;

        [ObservableProperty]
        private string _deviceConnectedStatus;

        [ObservableProperty]
        private bool _isDeviceConnected;

        [ObservableProperty]
        private bool _showAllMenus;

        [ObservableProperty] private DeviceStatus _deviceStatus;

        [ObservableProperty]
        private bool errorVisible;

        [ObservableProperty]
        private int _errorHeight;

        public BaseViewModel()
        {
            DeviceStatus = new DeviceStatus();
            //RefreshIconStatus();
            ShowAllMenus = true;
        }

        //public void RefreshIconStatus()
        //{
        //    int BatteryLevel = Cache.BatteryLevel;
        //    int daysRemaining = (Cache.SensorExpireDate > DateTime.Now) ? (int)(Cache.SensorExpireDate - DateTime.Now).TotalDays : 0;

        //    DeviceStatus.UpdateBattery(BatteryLevel);
        //    DeviceStatus.UpdateDevice(daysRemaining);
        //    DeviceStatus.UpdateSensor(daysRemaining);
        //    DeviceStatus.UpdateQualityControlExpiration(0);
        //    DeviceStatus.UpdatePressure(0);
        //    DeviceStatus.UpdateRelativeHumidity(0);
        //    DeviceStatus.UpdateTemperature(0);
        //}

        [RelayCommand]
        public async Task ExitToDashboard()
        {
            await Services.Navigation.ChooseTestView();
        }

        [RelayCommand]
        public async Task ExitToQC()
        {
            await Services.Navigation.QualityControlView();
        }

        public virtual void OnAppearing()
        {
            NewGlobalData();
        }

        public virtual void OnDisappearing()
        {
        }

        public virtual void NewGlobalData()
        {
            //RefreshIconStatus();
            DeviceSerialNumber = Services.Cache.DeviceSerialNumber;

            Firmware = Services.Cache.Firmware;
        }
    }
}
