 using FenomPlus.Enums;
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
    public partial class QCUserTestViewModel : BaseViewModel
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

        public QCUserTestViewModel()
        {
        }

        private async void Cache_BreathFlowChanged(object sender, EventArgs e)
        {
            if (Services.DeviceService.Current != null)
            {
                GaugeData = Services.DeviceService.Current.BreathFlow;
                GaugeSeconds = Services.DeviceService.Current.BreathManeuver.TimeRemaining;

                if (GaugeSeconds <= 0)
                {
                    if (Services.DeviceService.Current != null && Services.DeviceService.Current is BleDevice)
                    {
                        await Services.DeviceService.Current.StopTest();
                    }

                    await Services.Navigation.QCUserStopTestView();
                    return;
                }
            }

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

        public override void OnAppearing()
        {
            base.OnAppearing();

            // Allows Updating the Breath Gauge in UI
            if (Services.DeviceService.Current != null)
            {
                Services.DeviceService.Current.BreathFlowChanged += Cache_BreathFlowChanged;

                Services.DeviceService.Current?.IsNotConnectedRedirect();

                GaugeData = Services.DeviceService.Current.BreathFlow = 0;
            }

            GaugeSeconds = 10;
            GaugeStatus = "Start Blowing";
        }

        public override void OnDisappearing()
        {
            if (Services.DeviceService.Current != null)
                Services.DeviceService.Current.BreathFlowChanged -= Cache_BreathFlowChanged;

            base.OnDisappearing();
            PlaySounds.StopAll();
        }
    }
}
