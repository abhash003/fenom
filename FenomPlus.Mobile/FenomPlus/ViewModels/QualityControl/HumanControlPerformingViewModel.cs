using System;
using FenomPlus.Helpers;
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
            Services.Cache.BreathFlow = 0;
            TestSeconds = TestTime * (1000 / Services.Cache.BreathFlowTimer);
            Stop = false;

            Device.StartTimer(TimeSpan.FromMilliseconds(Services.Cache.BreathFlowTimer), () =>
            {
                TestSeconds--;
                TestTime = TestSeconds / (1000 / Services.Cache.BreathFlowTimer);

                GuageData = Services.Cache.BreathFlow;

                if (GuageData < Config.GaugeDataLow)
                {
                    GuageStatus = "Exhale Harder";
                }
                else if (GuageData > Config.GaugeDataHigh)
                {
                    GuageStatus = "Exhale Softer";
                }
                else
                {
                    GuageStatus = "Good Job!";
                }

                // return contiune of below the time
                GuageSeconds = TestSeconds / (1000 / Services.Cache.BreathFlowTimer);

                if ((TestSeconds <= 0) && (Stop == false))
                {
                    Services.BleHub.StopTest();

                    Services.Cache.HumanControlResult = GuageData;
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
        private int guageSeconds;
        public int GuageSeconds
        {
            get => guageSeconds;
            set
            {
                guageSeconds = value;
                OnPropertyChanged("GuageSeconds");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private float guageData;
        public float GuageData
        {
            get => guageData;
            set
            {
                guageData = value;
                OnPropertyChanged("GuageData");
                if (!Stop) PlaySounds.PlaySound(GuageData);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private string guageStatus;
        public string GuageStatus
        {
            get => guageStatus;
            set
            {
                guageStatus = value;
                OnPropertyChanged("GuageStatus");
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
