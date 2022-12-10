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
            if (DashboardViewModel.BleHub.IsNotConnectedRedirect())
            {
                if (!DeviceEnvironmentalWarning())
                {
                    return;
                }

                if (!DashboardViewModel.BleHub.ReadyForTest)
                {
                    DeviceNotReadyWarningProgress();
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
                if (!DeviceEnvironmentalWarning())
                {
                    return;
                }

                if (!DashboardViewModel.BleHub.ReadyForTest)
                {
                    DeviceNotReadyWarningProgress();                   
                    return;
                }

                DashboardViewModel.Cache.TestType = TestTypeEnum.Short;
                await DashboardViewModel.BleHub.StartTest(BreathTestEnum.Start6Second);
                await DashboardViewModel.Services.Navigation.BreathManeuverFeedbackView();
            }
        }

        private bool DeviceEnvironmentalWarning()
        {
            // Get the latest environmental info - updates Cache
            DashboardViewModel.BleHub.RequestEnvironmentalInfo();

            if (DashboardViewModel.Cache.EnvironmentalInfo.Humidity < Constants.HumidityLow18 ||
                DashboardViewModel.Cache.EnvironmentalInfo.Humidity > Constants.HumidityHigh92)
            {
                DashboardViewModel.Dialogs.ShowToast($"Humidity Level Out of Range: {DashboardViewModel.Cache.EnvironmentalInfo.Humidity}", 5);
                return false;
            }

            if (DashboardViewModel.Cache.EnvironmentalInfo.Pressure < Constants.PressureLow75 ||
                DashboardViewModel.Cache.EnvironmentalInfo.Pressure > Constants.PressureHigh110)
            {
                DashboardViewModel.Dialogs.ShowToast($"Pressure Level Out of Range: {DashboardViewModel.Cache.EnvironmentalInfo.Pressure}", 5);
                return false;
            }

            if (DashboardViewModel.Cache.EnvironmentalInfo.Temperature < Constants.TemperatureLow14 ||
                DashboardViewModel.Cache.EnvironmentalInfo.Temperature > Constants.TemperatureHigh35)
            {
                DashboardViewModel.Dialogs.ShowToast($"Temperature Level Out of Range: {DashboardViewModel.Cache.EnvironmentalInfo.Temperature}", 5);
                return false;
            }

            if (DashboardViewModel.Cache.EnvironmentalInfo.BatteryLevel < Constants.BatteryCritical3)
            {
                DashboardViewModel.Dialogs.ShowToast($"Battery Level is Critically Low: {DashboardViewModel.Cache.EnvironmentalInfo.BatteryLevel}", 5);
                return false;
            }

            return true;
        }

        private void DeviceNotReadyWarningToast()
        {
            int secondsRemaining = DashboardViewModel.BleHub.DeviceReadyCountDown;
            DashboardViewModel.Dialogs.ShowToast($"Device purging, please wait...", secondsRemaining);
        }

        private void DeviceNotReadyWarningProgress()
        {
            int secondsRemaining = DashboardViewModel.BleHub.DeviceReadyCountDown;
            DashboardViewModel.Dialogs.ShowSecondsProgress($"Device purging..", secondsRemaining);
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
