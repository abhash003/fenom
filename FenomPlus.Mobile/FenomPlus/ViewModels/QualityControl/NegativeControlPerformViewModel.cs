using FenomPlus.Services.DeviceService.Concrete;
using System;
using Xamarin.Forms;

namespace FenomPlus.ViewModels
{
    public class NegativeControlPerformViewModel : BaseViewModel
    {
        public NegativeControlPerformViewModel()
        {
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            TestTime = 10;
            TestSeconds = TestTime * (1000 / Services.Cache.BreathFlowTimer);
            Services.DeviceService.Current.BreathFlow = 0;
            Stop = false;

            Device.StartTimer(TimeSpan.FromMilliseconds(Services.Cache.BreathFlowTimer), () =>
            {
                TestSeconds--;
                TestTime = TestSeconds / (1000 / Services.Cache.BreathFlowTimer);
                if ((TestSeconds <= 0) && (Stop == false))
                {
                    _ = Services.DeviceService.Current.StopTest();
                    if (Services.DeviceService.Current.BreathFlow <= 0)
                    {
                        Services.Navigation.NegativeControlPassView();
                    }
                    else
                    {
                        Services.Navigation.NegativeControlFailView();
                    }
                }

                return (TestSeconds > 0) && (Stop == false);
            });
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
            Stop = true;
        }


        private int _TestTime;
        public int TestTime
        {
            get => _TestTime;
            set
            {
                _TestTime = value;
                OnPropertyChanged("TestTime");
            }
        }

        private bool Stop;
        private int TestSeconds;
        
    }
}
