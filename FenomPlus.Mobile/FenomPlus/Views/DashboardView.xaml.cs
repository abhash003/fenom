using System;
using Acr.UserDialogs;
using FenomPlus.ViewModels;
using FenomPlus.SDK.Core.Models;
using FenomPlus.Enums;

namespace FenomPlus.Views
{
    public partial class DashboardView : BaseContentPage
    {
        private readonly DashboardViewModel DashboardViewModel;

        public DashboardView()
        {
            InitializeComponent();
            BindingContext = DashboardViewModel = new DashboardViewModel();
        }

        private async void OnStandardTest(object sender, EventArgs e)
        {
            if (DashboardViewModel.BleHub.IsNotConnectedRedirect())
            {
                if (!DashboardViewModel.BleHub.ReadyForTest)
                {
                    int secondsRemaining = DashboardViewModel.BleHub.DeviceReadyCountDown;
                    DashboardViewModel.Dialogs.ShowToast($"Device is currently preparing for another test. An additional {secondsRemaining} seconds is required and this message will go away when the device is ready...", secondsRemaining);
                    return;
                }

                DashboardViewModel.Cache.TestType = TestTypeEnum.Standard;
                await DashboardViewModel.BleHub.StartTest(BreathTestEnum.Start10Second);
                await DashboardViewModel.Services.Navigation.BreathManeuverFeedbackView();
            }
        }

        private async void OnShortTest(object sender, EventArgs e)
        {
            if (DashboardViewModel.Services.BleHub.IsNotConnectedRedirect())
            {
                if (!DashboardViewModel.BleHub.ReadyForTest)
                {
                    int secondsRemaining = DashboardViewModel.BleHub.DeviceReadyCountDown;
                    DashboardViewModel.Dialogs.ShowToast($"Device is currently preparing for another test. An additional {secondsRemaining} seconds is required and this message will go away when the device is ready...", secondsRemaining);
                    return;
                }

                DashboardViewModel.Cache.TestType = TestTypeEnum.Short;
                await DashboardViewModel.BleHub.StartTest(BreathTestEnum.Start6Second);
                await DashboardViewModel.Services.Navigation.BreathManeuverFeedbackView();
            }
        }

        private async void OnTutorial(object sender, EventArgs e)
        {
            if (!DashboardViewModel.BleHub.ReadyForTest)
            {
                //await DisplayAlert("Not Ready", "Device is currently preparing for another test.  Please wait.", "OK");
                int secondsRemaining = DashboardViewModel.BleHub.DeviceReadyCountDown;
                DashboardViewModel.Dialogs.ShowToast($"Device is currently preparing for another test. An additional {secondsRemaining} seconds is required and this message will go away when the device is ready...", secondsRemaining);
                return;
            }

            await DashboardViewModel.Services.Navigation.TutorialView();
        }
        
        protected override void OnAppearing()
        {
            base.OnAppearing();
            DashboardViewModel.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            DashboardViewModel.OnDisappearing();
        }

        public override void NewGlobalData()
        {
            base.NewGlobalData();
            DashboardViewModel.NewGlobalData();
        }
    }
}
