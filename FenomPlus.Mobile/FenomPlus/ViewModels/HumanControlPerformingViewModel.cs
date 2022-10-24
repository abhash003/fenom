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
        override public void OnAppearing()
        {
            base.OnAppearing();
            TestTime = 10;
            Cache.BreathFlow = 0;
            TestSeconds = TestTime * (1000 / Cache.BreathFlowTimer);
            Stop = false;

            Device.StartTimer(TimeSpan.FromMilliseconds(Cache.BreathFlowTimer), () =>
            {
                TestSeconds--;
                TestTime = TestSeconds / (1000 / Cache.BreathFlowTimer);

                GuageData = Cache.BreathFlow;

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
                GuageSeconds = TestSeconds / (1000 / Cache.BreathFlowTimer);

                if ((TestSeconds <= 0) && (Stop == false))
                {
                    BleHub.StopTest();

                    Cache.HumanControlResult = GuageData;
                    Services.Navigation.HumanControlPreparingView();
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
        override public void NewGlobalData()
        {
            base.NewGlobalData();
        }
    }
}
