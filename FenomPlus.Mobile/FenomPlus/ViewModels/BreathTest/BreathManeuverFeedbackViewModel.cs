using FenomPlus.Enums;
using FenomPlus.Helpers;
using System;
using Xamarin.Forms;

namespace FenomPlus.ViewModels
{
    public class BreathManeuverFeedbackViewModel : BaseViewModel
    {
        private bool StartMeasure;

        public BreathManeuverFeedbackViewModel()
        {
        }

        private bool Stop;

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

        /// <summary>
        /// 
        /// </summary>
        public override void OnAppearing()
        {
            base.OnAppearing();
            Services.Device.IsNotConnectedRedirect();
            if (Services.Cache.TestType == TestTypeEnum.Standard)
            {
                TestType = "10-second Test";
                TestTime = 10;
            }
            else
            {
                TestType = "6-second Test";
                TestTime = 6;
            }

            Stop = false;
            StartMeasure = false;

            GuageData = 0;
            Services.Cache.BreathFlow = 0;
            TestGuageSeconds = TestTime * (1000 / Services.Cache.BreathFlowTimer);

            // setup our count down
            GuageSecondsCountdown = TestGuageSeconds;
            GuageSeconds = GuageSecondsCountdown / (1000 / Services.Cache.BreathFlowTimer);
            GuageStatus = "Start Blowing";

            // start timer
            Device.StartTimer(TimeSpan.FromMilliseconds(Services.Cache.BreathFlowTimer), () =>
            {
                GuageData = Services.Cache.BreathFlow;
                if ((GuageData <= 0.0f) && (StartMeasure == false))
                {
                    // return continue of below the time
                    
                    GuageStatus = "Start Blowing";
                }
                else
                {
                    TestGuageSeconds = Services.Cache.BreathManeuver.TimeRemaining;

                    if (GuageSecondsCountdown > 0) GuageSecondsCountdown--;
                    GuageSeconds = GuageSecondsCountdown / (1000 / Services.Cache.BreathFlowTimer);
                    StartMeasure = true;
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
                }

                // have we started the Measure yet?
                if (StartMeasure == true)
                {
                    if ((TestGuageSeconds <= 0) && (Stop == false))
                    {
                        Services.Device.StopTest();
                        Services.Navigation.StopExhalingView();
                    }
                }

                return (TestGuageSeconds > 0) && (Stop == false);
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

        private int TestGuageSeconds;

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

        public int GuageSecondsCountdown;

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
                if(!Stop) PlaySounds.PlaySound(GuageData);
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

        public override void NewGlobalData()
        {
            base.NewGlobalData();
        }
    }
}
