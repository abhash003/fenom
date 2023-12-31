﻿ using FenomPlus.Enums;
using FenomPlus.Helpers;
using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using FenomPlus.SDK.Core.Ble.Interface;
using Xamarin.Forms;
using Plugin.BLE.Abstractions.EventArgs;
using FenomPlus.Services.DeviceService.Concrete;

namespace FenomPlus.ViewModels
{
    public partial class BreathManeuverFeedbackViewModel : BaseViewModel
    {
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
            PlaySounds.PlaySound(GaugeData);
        }

        [ObservableProperty]
        private string _gaugeStatus;

        public BreathManeuverFeedbackViewModel()
        {
        }

        private void Cache_BreathFlowChanged(object sender, EventArgs e)
        {         

            if (Services.DeviceService.Current == null) return;

            GaugeData = Services.DeviceService.Current.BreathFlow;
            GaugeSeconds = Services.DeviceService.Current.BreathManeuver.TimeRemaining;

            if (GaugeSeconds <= 0)
            {
                Services.DeviceService.Current.StopTest();
                Services.Navigation.StopExhalingView();
            }
            else
            {
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
        }

        public override void OnAppearing()
        {
            base.OnAppearing();

            if (Services.DeviceService.Current != null)
            {
                // Allows Updating the Breath Gauge in UI
                Services.DeviceService.Current.BreathFlowChanged += Cache_BreathFlowChanged;

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

                GaugeData = Services.DeviceService.Current.BreathFlow = 0;
                GaugeSeconds = TestTime;
                GaugeStatus = "Start Blowing";
            }            
        }

        public override void OnDisappearing()
        {
            if (Services.DeviceService.Current != null)
            {
                Services.DeviceService.Current.BreathFlowChanged -= Cache_BreathFlowChanged;
            }            

            base.OnDisappearing();
            PlaySounds.StopAll();
        }
    }
}
