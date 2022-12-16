
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
            if (BleHub.IsNotConnectedRedirect())
            {
                if (!DeviceEnvironmentalWarning())
                {
                    return;
                }

                if (!BleHub.ReadyForTest)
                {
                    DeviceNotReadyWarningProgress();
                    return;
                }

                Cache.TestType = TestTypeEnum.Standard;
                await BleHub.StartTest(BreathTestEnum.Start10Second);
                await Services.Navigation.BreathManeuverFeedbackView();


            }
        }

        [RelayCommand]
        private async Task StartShortTest()
        {
            bool connected = BleHub.BleDevice.Connected;

            if (BleHub.IsNotConnectedRedirect())
            {
                if (!DeviceEnvironmentalWarning())
                {
                    return;
                }

                if (!BleHub.ReadyForTest)
                {
                    DeviceNotReadyWarningProgress();
                    return;
                }

                Cache.TestType = TestTypeEnum.Short;
                await BleHub.StartTest(BreathTestEnum.Start6Second);
                await Services.Navigation.BreathManeuverFeedbackView();
            }
        }

        private bool DeviceEnvironmentalWarning()
        {
            // Get the latest environmental info - updates Cache
            BleHub.RequestEnvironmentalInfo();

            if (Cache.EnvironmentalInfo.Humidity < Constants.HumidityLow18 ||
                Cache.EnvironmentalInfo.Humidity > Constants.HumidityHigh92)
            {
                Dialogs.ShowToast($"Humidity Level Out of Range: {Cache.EnvironmentalInfo.Humidity}", 5);
                return false;
            }

            if (Cache.EnvironmentalInfo.Pressure < Constants.PressureLow75 ||
                Cache.EnvironmentalInfo.Pressure > Constants.PressureHigh110)
            {
                Dialogs.ShowToast($"Pressure Level Out of Range: {Cache.EnvironmentalInfo.Pressure}", 5);
                return false;
            }

            if (Cache.EnvironmentalInfo.Temperature < Constants.TemperatureLow14 ||
                Cache.EnvironmentalInfo.Temperature > Constants.TemperatureHigh35)
            {
                Dialogs.ShowToast($"Temperature Level Out of Range: {Cache.EnvironmentalInfo.Temperature}", 5);
                return false;
            }

            if (Cache.EnvironmentalInfo.BatteryLevel < Constants.BatteryCritical3)
            {
                Dialogs.ShowToast($"Battery Level is Critically Low: {Cache.EnvironmentalInfo.BatteryLevel}", 5);
                return false;
            }

            return true;
        }

        private void DeviceNotReadyWarningProgress()
        {
            Dialogs.ShowSecondsProgress($"Device purging..", BleHub.DeviceReadyCountDown);
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            Services.BleHub.IsConnected();
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
