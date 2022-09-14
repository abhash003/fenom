using FenomPlus.Helpers;
using FenomPlus.Models;
using FenomPlus.Views;
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
            //Services.BleHub.IsConnected();
            if (Cache.TestType == TestTypeEnum.Standard)
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
            Cache.BreathFlow = 0;
            TestGuageSeconds = TestTime * (1000 / Cache.BreathFlowTimer);
            GuageSeconds = TestGuageSeconds / (1000 / Cache.BreathFlowTimer);
            GuageStatus = "Start Blowing";

            // start timer
            Device.StartTimer(TimeSpan.FromMilliseconds(Cache.BreathFlowTimer), () =>
            {
                GuageData = Cache.BreathFlow;
                if ((GuageData <= 0.0f) && (StartMeasure == false))
                {
                    // return contiune of below the time
                    
                    GuageStatus = "Start Blowing";
                }
                else
                {
                    TestGuageSeconds = Cache._BreathManeuver.TimeRemaining;
                    GuageSeconds = TestGuageSeconds / (1000 / Cache.BreathFlowTimer);
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
                        BleHub.StopTest();
                        Shell.Current.GoToAsync(new ShellNavigationState($"///{nameof(StopExhalingView)}"), false);
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

        /// <summary>
        /// 
        /// </summary>
        override public void NewGlobalData()
        {
            base.NewGlobalData();
        }
    }
}
