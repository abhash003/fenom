using FenomPlus.Enums;
using FenomPlus.Helpers;
using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Xamarin.Forms;

namespace FenomPlus.ViewModels
{
    public partial class BreathManeuverFeedbackViewModel : BaseViewModel
    {
        private bool StartMeasure;
        private int TestGaugeSeconds;

        private bool Stop;
        public int GaugeSecondsCountdown;

        [ObservableProperty]
        private string _testType;

        [ObservableProperty]
        private int _testTime;


        [ObservableProperty]
        private int _gaugeSeconds;

        [ObservableProperty]
        private float _gaugeData;

        partial void OnGaugeDataChanged(float value)
        {
            if (!Stop)
                PlaySounds.PlaySound(GaugeData);
        }

        [ObservableProperty]
        private string _gaugeStatus;

        public BreathManeuverFeedbackViewModel()
        {
        }


        public override void OnAppearing()
        {
            base.OnAppearing();
            Services.DeviceService.Current.IsNotConnectedRedirect();
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

            GaugeData = 0;
            Services.Cache.BreathFlow = 0;
            TestGaugeSeconds = TestTime * (1000 / Services.Cache.BreathFlowTimer);

            // setup our count down
            GaugeSecondsCountdown = TestGaugeSeconds;
            GaugeSeconds = GaugeSecondsCountdown / (1000 / Services.Cache.BreathFlowTimer);
            GaugeStatus = "Start Blowing";

            // start timer
            Device.StartTimer(TimeSpan.FromMilliseconds(Services.Cache.BreathFlowTimer), () =>
            {
                GaugeData = Services.Cache.BreathFlow;
                if ((GaugeData <= 0.0f) && (StartMeasure == false))
                {
                    // return continue of below the time

                    GaugeStatus = "Start Blowing";
                }
                else
                {
                    TestGaugeSeconds = Services.Cache.BreathManeuver.TimeRemaining;

                    if (GaugeSecondsCountdown > 0) GaugeSecondsCountdown--;
                    GaugeSeconds = GaugeSecondsCountdown / (1000 / Services.Cache.BreathFlowTimer);
                    StartMeasure = true;
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
                }

                // have we started the Measure yet?
                if (StartMeasure == true)
                {
                    if ((TestGaugeSeconds <= 0) && (Stop == false))
                    {
                        Services.DeviceService.Current.StopTest();
                        Services.Navigation.StopExhalingView();
                    }
                }

                return (TestGaugeSeconds > 0) && (Stop == false);
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

        public override void NewGlobalData()
        {
            base.NewGlobalData();
        }
    }
}