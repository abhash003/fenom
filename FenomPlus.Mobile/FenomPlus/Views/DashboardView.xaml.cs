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
            if (DashboardViewModel.Services.Device.IsNotConnectedRedirect())
            {
                if (!DashboardViewModel.Services.Device.ReadyForTest)
                {
                    DeviceNotReadyWarning1();
                    return;
                }

                DashboardViewModel.Services.Cache.TestType = TestTypeEnum.Standard;
                await DashboardViewModel.Services.Device.StartTest(BreathTestEnum.Start10Second);
                await DashboardViewModel.Services.Navigation.BreathManeuverFeedbackView();
            }
        }

        private async void OnShortTest(object sender, EventArgs e)
        {
            if (DashboardViewModel.Services.Device.IsNotConnectedRedirect())
            {
                if (!DashboardViewModel.Services.Device.ReadyForTest)
                {
                    DeviceNotReadyWarning2();                   
                    return;
                }

                DashboardViewModel.Services.Cache.TestType = TestTypeEnum.Short;
                await DashboardViewModel.Services.Device.StartTest(BreathTestEnum.Start6Second);
                await DashboardViewModel.Services.Navigation.BreathManeuverFeedbackView();
            }
        }

        private void DeviceNotReadyWarning1()
        {
            if (!DashboardViewModel.Services.Device.ReadyForTest)
            {
                int secondsRemaining = DashboardViewModel.Services.Device.DeviceReadyCountDown;
                DashboardViewModel.Dialogs.ShowToast($"Preparing for test. {secondsRemaining} seconds required.", secondsRemaining);
            }
        }

        private void DeviceNotReadyWarning2()
        {
            if (!DashboardViewModel.Services.Device.ReadyForTest)
            {
                int secondsRemaining = DashboardViewModel.Services.Device.DeviceReadyCountDown;
                DashboardViewModel.Dialogs.ShowSecondsProgress($"Preparing for test...", secondsRemaining);
            }
        }

        private async void OnTutorial(object sender, EventArgs e)
        {
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
