using System;
using FenomPlus.Helpers;
using FenomPlus.Services.DeviceService.Concrete;
using Xamarin.Forms;

namespace FenomPlus.ViewModels
{
    public class HumanControlPerformingViewModel : BaseViewModel
    {
        public HumanControlPerformingViewModel()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnAppearing()
        {
            base.OnAppearing();
            TestTime = 10;
            Services.DeviceService.Current.BreathFlow = 0;
            TestSeconds = TestTime * (1000 / Services.Cache.BreathFlowTimer);
            Stop = false;

            Device.StartTimer(TimeSpan.FromMilliseconds(Services.Cache.BreathFlowTimer), () =>
            {
                TestSeconds--;
                TestTime = TestSeconds / (1000 / Services.Cache.BreathFlowTimer);

                GaugeData = Services.DeviceService.Current.BreathFlow;

                if (GaugeData < Config.GaugeDataLow)
                {
                    GaugeStatus = "Exhale Harder";
                }
                else if (GaugeData > Config.GaugeDataHigh)
                {
                    GaugeStatus = "Exhale Softer";
                }
                else
                {
                    GaugeStatus = "Good Job!";
                }

                // return contiune of below the time
                GaugeSeconds = TestSeconds / (1000 / Services.Cache.BreathFlowTimer);

                if ((TestSeconds <= 0) && (Stop == false))
                {
                    _ = (Services.DeviceService.Current as BleDevice).StopTest();

                    Services.Cache.HumanControlResult = GaugeData;
                    Services.Navigation.HumanControlPreparingView();
                }
                return (TestSeconds > 0) && (Stop == false);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnDisappearing()
        {
            base.OnDisappearing();
            Stop = true;
            PlaySounds.StopAll();
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
        private int gaugeSeconds;
        public int GaugeSeconds
        {
            get => gaugeSeconds;
            set
            {
                gaugeSeconds = value;
                OnPropertyChanged("GaugeSeconds");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private float gaugeData;
        public float GaugeData
        {
            get => gaugeData;
            set
            {
                gaugeData = value;
                OnPropertyChanged("GaugeData");
                if (!Stop) PlaySounds.PlaySound(GaugeData);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private string gaugeStatus;
        public string GaugeStatus
        {
            get => gaugeStatus;
            set
            {
                gaugeStatus = value;
                OnPropertyChanged("GaugeStatus");
            }
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
