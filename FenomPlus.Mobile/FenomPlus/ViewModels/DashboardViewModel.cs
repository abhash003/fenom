
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using FenomPlus.Enums;
using FenomPlus.SDK.Core.Models;
using FenomPlus.Services;
using FenomPlus.Services.DeviceService.Concrete;
using FenomPlus.Services.DeviceService.Enums;
using Syncfusion.Drawing;

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
                DeviceCheckEnum deviceStatus = Services.DeviceService.Current.CheckDeviceBeforeTest();

                switch (deviceStatus)
                {
                    case DeviceCheckEnum.Ready:
                        Services.Cache.TestType = TestTypeEnum.Standard;
                        await Services.DeviceService.Current.StartTest(BreathTestEnum.Start10Second);
                        await Services.Navigation.BreathManeuverFeedbackView();
                        break;
                    case DeviceCheckEnum.DevicePurging:
                        await Services.Dialogs.ShowSecondsProgressAsync($"Device purging..", Services.DeviceService.Current.DeviceReadyCountDown);
                        break;
                    case DeviceCheckEnum.HumidityOutOfRange:
                        Services.Dialogs.ShowAlert($"Unable to run test. Humidity level ({Services.DeviceService.Current.EnvironmentalInfo.Humidity}%) is out of range.", "Humidity Warning", "Close");
                        break;
                    case DeviceCheckEnum.PressureOutOfRange:
                        Services.Dialogs.ShowAlert($"Unable to run test. Pressure level ({Services.DeviceService.Current.EnvironmentalInfo.Pressure} kPa) is out of range.", "Pressure Warning", "Close");
                        break;
                    case DeviceCheckEnum.TemperatureOutOfRange:
                        Services.Dialogs.ShowAlert($"Unable to run test. Temperature level ({Services.DeviceService.Current.EnvironmentalInfo.Temperature} °C) is out of range.","Temperature Warning", "Close");
                        break;
                    case DeviceCheckEnum.BatteryCriticallyLow:
                        Services.Dialogs.ShowAlert($"Unable to run test. Battery Level ({Services.DeviceService.Current.EnvironmentalInfo.BatteryLevel}%) is critically low: ", "Battery Warning","Close");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        [RelayCommand]
        private async Task StartShortTest()
        {
            if (Services.DeviceService.Current != null && Services.DeviceService.Current.IsNotConnectedRedirect())
            {
                switch (Services.DeviceService.Current.CheckDeviceBeforeTest())
                {
                    case DeviceCheckEnum.Ready:
                        Services.Cache.TestType = TestTypeEnum.Short;
                        await Services.DeviceService.Current.StartTest(BreathTestEnum.Start6Second);
                        await Services.Navigation.BreathManeuverFeedbackView();
                        break;
                    case DeviceCheckEnum.DevicePurging:
                        await Services.Dialogs.ShowSecondsProgressAsync($"Device purging..", Services.DeviceService.Current.DeviceReadyCountDown);
                        break;
                    case DeviceCheckEnum.HumidityOutOfRange:
                        Services.Dialogs.ShowAlert($"Humidity level ({Services.DeviceService.Current.EnvironmentalInfo.Humidity}%) is out of range.", "Unable to Run Test", "Close");
                        break;
                    case DeviceCheckEnum.PressureOutOfRange:
                        Services.Dialogs.ShowAlert($"Pressure level ({Services.DeviceService.Current.EnvironmentalInfo.Pressure} kPa) is out of range.", "Unable to Run Test", "Close");
                        break;
                    case DeviceCheckEnum.TemperatureOutOfRange:
                        Services.Dialogs.ShowAlert($"Temperature level ({Services.DeviceService.Current.EnvironmentalInfo.Temperature} °C) is out of range.", "Unable to Run Test", "Close");
                        break;
                    case DeviceCheckEnum.BatteryCriticallyLow:
                        Services.Dialogs.ShowAlert($"Battery Level ({Services.DeviceService.Current.EnvironmentalInfo.BatteryLevel}%) is critically low.", "Unable to Run Test", "Close");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
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
