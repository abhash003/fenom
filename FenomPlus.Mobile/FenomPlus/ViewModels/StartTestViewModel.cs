using FenomPlus.Enums;
using FenomPlus.SDK.Core.Models;
using Xamarin.Forms;

namespace FenomPlus.ViewModels
{
    public class StartTestViewModel : BaseViewModel
    {
        public StartTestViewModel()
        {
        }

        private string _TestType;
        public string TestType
        {
            get => _TestType;
            set
            {
                _TestType = value;
                OnPropertyChanged("TestType");
            }
        }


        private ImageSource _TestImageSource;
        public ImageSource TestImageSource
        {
            get => _TestImageSource;
            set
            {
                _TestImageSource = value;
                OnPropertyChanged("TestImageSource");
            }
        }

        private string _TestSeconds;
        public string TestSeconds
        {
            get => _TestSeconds;
            set
            {
                _TestSeconds = value;
                OnPropertyChanged("TestSeconds");
            }
        }

        private string _TestButton;
        public string TestButton
        {
            get => _TestButton;
            set
            {
                _TestButton = value;
                OnPropertyChanged("TestButton");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnAppearing()
        {
            base.OnAppearing();
            Services.BleHub.IsNotConnectedRedirect();
            if (Cache.TestType == TestTypeEnum.Standard) {
                TestType = "10-second Test";
                TestButton = "Take 10-second";
                TestSeconds = "10 seconds";
                TestImageSource = "StandardBreathe";
            } else {
                TestType = "6-second Test";
                TestButton = "Take 6 seconds";
                TestSeconds = "6 seconds";
                TestImageSource = "ShortBreathe";
            }

            BleHub.StartTest(
                (Cache.TestType == TestTypeEnum.Standard) ?
                BreathTestEnum.Start10Second :
                BreathTestEnum.Start6Second);
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
