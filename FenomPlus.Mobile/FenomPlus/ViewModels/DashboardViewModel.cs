
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
            if (Services.DeviceService.Current != null && Services.DeviceService.Current.IsNotConnectedRedirect())
            {
                if (!Services.Cache.CheckDeviceBeforeTest())
                {
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
            if (Services.DeviceService.Current != null && Services.DeviceService.Current.IsNotConnectedRedirect())
            {
                if (!Services.Cache.CheckDeviceBeforeTest())
                {
                    return;
                }

                Services.Cache.TestType = TestTypeEnum.Short;
                await Services.DeviceService.Current.StartTest(BreathTestEnum.Start6Second);
                await Services.Navigation.BreathManeuverFeedbackView();
            }
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
