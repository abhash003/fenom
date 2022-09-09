using System;
using System.Windows.Input;
using FenomPlus.Enums;
using FenomPlus.Helpers;
using FenomPlus.Models;
using FenomPlus.SDK.Core.Models;
using Xamarin.Forms;

namespace FenomPlus.ViewModels
{
    public class ChooseTestViewModel : BaseViewModel
    {
        public ICommand DismissCommand { get; private set; }

        public ChooseTestViewModel()
        {
            DeviceStatus = new DeviceStatus();

            ErrorList = new RangeObservableCollection<Alert>();
            
            Cache.BatteryStatus = false;
            Cache.DeviceSensorExpiring = false;
            Cache.DeviceExpiring = false;

            RefreshErrorList();
            DismissCommand = new Command<Alert>((model) => {
                foreach(Alert alert in ErrorList)
                {
                    if (model.Id != alert.Id) continue;

                    if(model.Id == (int)AlertEnum.Battery)
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
        }

        /// <summary>
        /// 
        /// </summary>
        public void RefreshErrorList()
        {
            ErrorList.Clear();

            int BatteryLevel = Cache.BatteryLevel;
            DeviceStatus.ResetBarColor();
            BatteryLevel = 12;

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
            UpdateErrorList();
        }

        /// <summary>
        /// 
        /// </summary>
        override public void OnAppearing()
        {
            base.OnAppearing();
            //Services.BleHub.IsConnected();
            RefreshErrorList();
        }

        /// <summary>
        /// 
        /// </summary>
        override public void OnDisappearing()
        {
            base.OnDisappearing();
        }


        public DeviceStatus DeviceStatus { get; set; }

        public RangeObservableCollection<Alert> ErrorList { get;set; }

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
        override public void NewGlobalData()
        {
            base.NewGlobalData();
        }
    }
}
