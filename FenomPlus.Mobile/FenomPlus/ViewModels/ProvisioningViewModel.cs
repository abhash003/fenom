
using System;

namespace FenomPlus.ViewModels
{
    public class ProvisioningViewModel : BaseViewModel
    {
        public ProvisioningViewModel()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        override public void OnAppearing()
        {
            base.OnAppearing();
            // get GMT
            //Date = DateTime.UtcNow.ToString("MM/dd/yyyy");
            Date = DateTime.UtcNow.ToString("yyyy-MM-dd");
            Time = DateTime.UtcNow.ToString("HH:mm:ss");
            SerialNumber = Cache.DeviceSerialNumber.Replace("F150-","");
        }

        /// <summary>
        /// 
        /// </summary>
        override public void OnDisappearing()
        {
            base.OnDisappearing();
        }

        /// <summary>
        /// 
        /// </summary>
        override public void NewGlobalData()
        {
            base.NewGlobalData();
        }


        
        private string _SerialNumber;
        public string SerialNumber
        {
            get => _SerialNumber;
            set
            {
                _SerialNumber = value;
                OnPropertyChanged("SerialNumber");
            }
        }

        private string _Date;
        public string Date
        {
            get => _Date;
            set
            {
                _Date = value;
                OnPropertyChanged("Date");
            }
        }

        private string _Time;
        public string Time
        {
            get => _Time;
            set
            {
                _Time = value;
                OnPropertyChanged("Time");
            }
        }
    }
}
