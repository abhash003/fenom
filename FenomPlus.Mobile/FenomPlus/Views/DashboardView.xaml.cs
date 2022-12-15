using System;
using Acr.UserDialogs;
using FenomPlus.ViewModels;
using FenomPlus.SDK.Core.Models;
using FenomPlus.Enums;
using Xamarin.Forms.Xaml;

namespace FenomPlus.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
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
            if (DashboardViewModel.Services.DeviceService.Current.IsNotConnectedRedirect())
            {
                if (!DashboardViewModel.Services.DeviceService.Current.ReadyForTest)
                {
                    DeviceNotReadyWarning1();
                    return;
                }

                DashboardViewModel.Services.Cache.TestType = TestTypeEnum.Standard;
                DashboardViewModel.Services.DeviceService.Current.StartTest(BreathTestEnum.Start10Second);
                await DashboardViewModel.Services.Navigation.BreathManeuverFeedbackView();
            }
        }

        private async void OnShortTest(object sender, EventArgs e)
        {
            if (DashboardViewModel.Services.DeviceService.Current.IsNotConnectedRedirect())
            {
                if (!DashboardViewModel.Services.DeviceService.Current.ReadyForTest)
                {
                    DeviceNotReadyWarning2();                   
                    return;
                }

                DashboardViewModel.Services.Cache.TestType = TestTypeEnum.Short;
                DashboardViewModel.Services.DeviceService.Current.StartTest(BreathTestEnum.Start6Second);
                await DashboardViewModel.Services.Navigation.BreathManeuverFeedbackView();
            }
        }

        private void DeviceNotReadyWarning1()
        {
            if (!DashboardViewModel.Services.DeviceService.Current.ReadyForTest)
            {
                int secondsRemaining = DashboardViewModel.Services.DeviceService.Current.DeviceReadyCountDown;
                DashboardViewModel.Dialogs.ShowToast($"Preparing for test. {secondsRemaining} seconds required.", secondsRemaining);
            }
        }

        private void DeviceNotReadyWarning2()
        {
            if (!DashboardViewModel.Services.DeviceService.Current.ReadyForTest)
            {
                int secondsRemaining = DashboardViewModel.Services.DeviceService.Current.DeviceReadyCountDown;
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
            if (DashboardViewModel.Dialogs.SecondsProgressDialogShowing())
            {
                DashboardViewModel.Dialogs.DismissSecondsProgressDialog();
            }

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