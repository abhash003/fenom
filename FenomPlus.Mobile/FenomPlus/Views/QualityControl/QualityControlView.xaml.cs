using System;
using Acr.UserDialogs;
using FenomPlus.ViewModels;
using FenomPlus.SDK.Core.Models;
using FenomPlus.Enums;

namespace FenomPlus.Views
{
    public partial class QualityControlView : BaseContentPage
    {
        private readonly DashboardViewModel DashboardViewModel;

        public QualityControlView()
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
                    DeviceNotReadyWarning1();
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
                    DeviceNotReadyWarning2();                   
                    return;
                }

                DashboardViewModel.Cache.TestType = TestTypeEnum.Short;
                await DashboardViewModel.BleHub.StartTest(BreathTestEnum.Start6Second);
                await DashboardViewModel.Services.Navigation.BreathManeuverFeedbackView();
            }
        }

        private void DeviceNotReadyWarning1()
        {
            if (!DashboardViewModel.BleHub.ReadyForTest)
            {
                int secondsRemaining = DashboardViewModel.BleHub.DeviceReadyCountDown;
                DashboardViewModel.Dialogs.ShowToast($"Preparing for test. {secondsRemaining} seconds required.", secondsRemaining);
            }
        }

        private void DeviceNotReadyWarning2()
        {
            if (!DashboardViewModel.BleHub.ReadyForTest)
            {
                int secondsRemaining = DashboardViewModel.BleHub.DeviceReadyCountDown;
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
