
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using FenomPlus.Enums;
using FenomPlus.SDK.Core.Ble.PluginBLE;
using FenomPlus.SDK.Core.Models;

namespace FenomPlus.ViewModels
{
    public partial class DashboardViewModel : BaseViewModel
    {

        public DashboardViewModel()
        {
        }

        [RelayCommand]
        private async Task StartStandardTest()
        {
            if (Services.DeviceService.Current.IsNotConnectedRedirect())
            {
                if (!DeviceEnvironmentalWarning())
                {
                    await Dialogs.ShowAlertAsync($"Not receiving valid temperature, humidity, pressure or battery level.", "Environment Error!", "Exit");
                    return;
                }

                if (!Services.DeviceService.Current.ReadyForTest)
                {
                    DeviceNotReadyWarningProgress();
                    return;
                }

                Services.Cache.TestType = TestTypeEnum.Standard;
                await Services.DeviceService.Current.StartTest(BreathTestEnum.Start10Second);
                await Services.Navigation.BreathManeuverFeedbackView();


            }
        }

        [RelayCommand]
        private async Task StartShortTest()
        {
            bool connected = Services.DeviceService.Current.Connected;

            if (Services.DeviceService.Current.IsNotConnectedRedirect())
            {
                if (!DeviceEnvironmentalWarning())
                {
                    await Dialogs.ShowAlertAsync($"Not receiving valid temperature, humidity, pressure or battery level.", "Environment Error!", "Exit");
                    return;
                }

                if (!Services.DeviceService.Current.ReadyForTest)
                {
                    DeviceNotReadyWarningProgress();
                    return;
                }

                Services.Cache.TestType = TestTypeEnum.Short;
                await Services.DeviceService.Current.StartTest(BreathTestEnum.Start6Second);
                await Services.Navigation.BreathManeuverFeedbackView();
            }
        }

        private bool DeviceEnvironmentalWarning()
        {
            // Get the latest environmental info - updates Cache
            Services.DeviceService.Current.RequestEnvironmentalInfo();

            if (Services.Cache.EnvironmentalInfo.Humidity < Constants.HumidityLow18 ||
                Services.Cache.EnvironmentalInfo.Humidity > Constants.HumidityHigh92)
            {
                Dialogs.ShowToast($"Humidity Level Out of Range: {Services.Cache.EnvironmentalInfo.Humidity}", 5);
                return false;
            }

            if (Services.Cache.EnvironmentalInfo.Pressure < Constants.PressureLow75 ||
                Services.Cache.EnvironmentalInfo.Pressure > Constants.PressureHigh110)
            {
                Dialogs.ShowToast($"Pressure Level Out of Range: {Services.Cache.EnvironmentalInfo.Pressure}", 5);
                return false;
            }

            if (Services.Cache.EnvironmentalInfo.Temperature < Constants.TemperatureLow14 ||
                Services.Cache.EnvironmentalInfo.Temperature > Constants.TemperatureHigh35)
            {
                Dialogs.ShowToast($"Temperature Level Out of Range: {Services.Cache.EnvironmentalInfo.Temperature}", 5);
                return false;
            }

            if (Services.Cache.EnvironmentalInfo.BatteryLevel < Constants.BatteryCritical3)
            {
                Dialogs.ShowToast($"Battery Level is Critically Low: {Services.Cache.EnvironmentalInfo.BatteryLevel}", 5);
                return false;
            }

            return true;
        }

        private void DeviceNotReadyWarningProgress()
        {
            Dialogs.ShowSecondsProgress($"Device purging..", Services.DeviceService.Current.DeviceReadyCountDown);
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            //Services.Device.IsConnected();
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        public override void NewGlobalData()
        {
            base.NewGlobalData();
        }
    }
}
