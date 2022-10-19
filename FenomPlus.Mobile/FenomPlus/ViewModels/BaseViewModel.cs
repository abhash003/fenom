using FenomPlus.Database.Repository.Interfaces;
using FenomPlus.Enums;
using FenomPlus.Helpers;
using FenomPlus.Interfaces;
using FenomPlus.Models;
using FenomPlus.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;

namespace FenomPlus.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged, IBaseServices
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

        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        private string title = string.Empty;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        private string deviceSerialNumber;
        public string DeviceSerialNumber
        {
            get { return deviceSerialNumber; }
            set { SetProperty(ref deviceSerialNumber, value);  }
        }

        private string firmware;
        public string Firmware
        {
            get { return firmware; }
            set { SetProperty(ref firmware, value); }
        }

        private string deviceConnectedStatus;
        public string DeviceConnectedStatus
        {
            get { return deviceConnectedStatus; }
            set { SetProperty(ref deviceConnectedStatus, value); }
        }
        

        private bool isDeviceConnected;
        public bool IsDeviceConnected
        {
            get { return isDeviceConnected; }
            set { SetProperty(ref isDeviceConnected, value); }
        }

        private bool showAllMenus;
        public bool ShowAllMenus
        {
            get { return showAllMenus; }
            set { SetProperty(ref showAllMenus, value); }
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="backingStore"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <param name="onChanged"></param>
        /// <returns></returns>
        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public ICommand DismissCommand { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public BaseViewModel()
        {
            DeviceStatus = new DeviceStatus();

            ErrorList = new RangeObservableCollection<Alert>();
            RefreshErrorList();
            DismissCommand = new Command<Alert>((model) => {
                foreach (Alert alert in ErrorList)
                {
                    if (model.Id != alert.Id) continue;

                    if (model.Id == (int)AlertEnum.Battery)
                    {
                        Cache.BatteryStatus = true;
                    }

                    if (model.Id == (int)AlertEnum.DeviceSensor)
                    {
                        Cache.DeviceSensorExpiring = true;
                    }

                    if (model.Id == (int)AlertEnum.Device)
                    {
                        Cache.DeviceExpiring = true;
                    }

                    ErrorList.Remove(alert);
                    break;
                }
                UpdateErrorList();
            });
            ShowAllMenus = true;
            NewGlobalData();
        }

        /// <summary>
        /// 
        /// </summary>
        public void RefreshErrorList()
        {
            ErrorList.Clear();

            int BatteryLevel = Cache.BatteryLevel;
            DeviceStatus.ResetBarColor();
            BatteryLevel = 75;

            SensorStatus _BatterySensor = DeviceStatus.UpdateBatteryDevice(BatteryLevel);

            if (Cache.BatteryStatus == false)
            {
                if (BatteryLevel <= Config.BatteryLevelLow)
                {
                    int TestsRemaining = BatteryLevel / 3;
                    ErrorList.Add(new Alert()
                    {
                        Id = (int)AlertEnum.Battery,
                        Description = string.Format("Fenom Plus has {0}% charge with {1} tests remaining. Please connect your device to the charging port.", BatteryLevel, TestsRemaining),
                        Image = _BatterySensor.ImageName,
                        Title = "Device Battery Low"
                    });
                }
            }
            else if (BatteryLevel > Config.BatteryLevelLow)
            {
                Cache.BatteryStatus = false;
            }

            /*
            int daysRemaining = (Cache.SensorExpireDate > DateTime.Now) ? (int)(Cache.SensorExpireDate - DateTime.Now).TotalDays : 0;

            DeviceStatus.UpdateDeviceExpiration(daysRemaining);
            DeviceStatus.UpdateSensoryExpiration(daysRemaining);
            if (daysRemaining <= Config.DaysRemaining)
            {
                if (Cache.DeviceSensorExpiring == false)
                {
                    ErrorList.Add(new Alert()
                    {
                        Id = (int)AlertEnum.DeviceSensor,
                        Description = string.Format("Fenom Plus sensor will expire in {0} days. For information on ordering a replacement sensor and how to replace your sensor, please view online FAQ.", daysRemaining),
                        Image = "SensorWarning",
                        Title = "Device Sensor Expiring Soon"
                    });
                }

                if (Cache.DeviceExpiring == false)
                {
                    ErrorList.Add(new Alert()
                    {
                        Id = (int)AlertEnum.Device,
                        Description = string.Format("Fenom Plus Device will expire in {0} days. For information on ordering a replacement device, please view online FAQ.", daysRemaining),
                        Image = "DeviceWarning",
                        Title = "Device Expiring Soon"
                    });
                }
            }
            else if (daysRemaining > Config.DaysRemaining)
            {
                Cache.DeviceSensorExpiring = false;
                Cache.DeviceExpiring = false;
            }

            // calucalte quaility contro lexpiration here
            DeviceStatus.UpdateQualityControlExpiration(0);
            */

            if(Services.BleHub.IsConnected())
            {
                if (Cache.FenomReady)
                {
                    DeviceStatus.UpdateDeviceState(2); // green
                } else {
                    DeviceStatus.UpdateDeviceState(1); // yellow
                }
            } else {
                DeviceStatus.UpdateDeviceState(0); // red
            }

            UpdateErrorList();
        }

        public DeviceStatus DeviceStatus { get; set; }

        public RangeObservableCollection<Alert> ErrorList { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public void UpdateErrorList()
        {
            ErrorHeight = ErrorList.Count * 74;
            ErrorVisable = ErrorHeight > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        private bool errorVisable;
        public bool ErrorVisable
        {
            get => errorVisable;
            set
            {
                errorVisable = value;
                OnPropertyChanged("ErrorVisable");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private int errorHeight;
        public int ErrorHeight
        {
            get => errorHeight;
            set
            {
                errorHeight = value;
                OnPropertyChanged("ErrorHeight");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        virtual public void OnAppearing()
        {
            NewGlobalData();
            //IsDeviceConnected = Services.BleHub.IsNotConnectedRedirect();
        }

        /// <summary>
        /// 
        /// </summary>
        virtual public void OnDisappearing()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        virtual public void NewGlobalData()
        {
            RefreshErrorList();
            DeviceSerialNumber = Services.Cache.DeviceSerialNumber;
            Firmware = Services.Cache.Firmware;
            DeviceConnectedStatus = Services.Cache.DeviceConnectedStatus;
        }
    }
}
