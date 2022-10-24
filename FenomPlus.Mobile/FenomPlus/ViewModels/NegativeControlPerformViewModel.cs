using System;
using FenomPlus.Views;
using Xamarin.Forms;

namespace FenomPlus.ViewModels
{
    public class NegativeControlPerformViewModel : BaseViewModel
    {
        public NegativeControlPerformViewModel()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        override public void OnAppearing()
        {
            base.OnAppearing();
            TestTime = 10;
            TestSeconds = TestTime * (1000 / Cache.BreathFlowTimer);
            Cache.BreathFlow = 0;
            Stop = false;

            Device.StartTimer(TimeSpan.FromMilliseconds(Cache.BreathFlowTimer), () =>
            {
                TestSeconds--;
                TestTime = TestSeconds / (1000 / Cache.BreathFlowTimer);
                if ((TestSeconds <= 0) && (Stop == false))
                {
                    BleHub.StopTest();
                    if (Cache.BreathFlow <= 0)
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

        /// <summary>
        /// 
        /// </summary>
        override public void OnDisappearing()
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

        /// <summary>
        /// 
        /// </summary>
        override public void NewGlobalData()
        {
            base.NewGlobalData();
        }
    }
}
