
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using FenomPlus.Enums;
using FenomPlus.SDK.Core.Models;
using FenomPlus.Services;
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
                CacheService.DeviceCheckEnum deviceStatus = Services.Cache.CheckDeviceBeforeTest();

                switch (deviceStatus)
                {
                    case CacheService.DeviceCheckEnum.Ready:
                        Services.Cache.TestType = TestTypeEnum.Standard;
                        await Services.DeviceService.Current.StartTest(BreathTestEnum.Start10Second);
                        await Services.Navigation.BreathManeuverFeedbackView();
                        break;
                    case CacheService.DeviceCheckEnum.DevicePurging:
                        await Services.Dialogs.ShowSecondsProgressAsync($"Device purging..", Services.DeviceService.Current.DeviceReadyCountDown);
                        break;
                    case CacheService.DeviceCheckEnum.HumidityOutOfRange:
                        Services.Dialogs.ShowAlert($"Unable to run test. Humidity level ({Services.Cache.EnvironmentalInfo.Humidity}%) is out of range.", "Humidity Warning", "Close");
                        break;
                    case CacheService.DeviceCheckEnum.PressureOutOfRange:
                        Services.Dialogs.ShowAlert($"Unable to run test. Pressure level ({Services.Cache.EnvironmentalInfo.Pressure} kPa) is out of range.", "Pressure Warning", "Close");
                        break;
                    case CacheService.DeviceCheckEnum.TemperatureOutOfRange:
                        Services.Dialogs.ShowAlert($"Unable to run test. Temperature level ({Services.Cache.EnvironmentalInfo.Temperature} °C) is out of range.","Temperature Warning", "Close");
                        break;
                    case CacheService.DeviceCheckEnum.BatteryCriticallyLow:
                        Services.Dialogs.ShowAlert($"Unable to run test. Battery Level ({Services.Cache.EnvironmentalInfo.BatteryLevel}%) is critically low: ", "Battery Warning","Close");
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
                switch (Services.Cache.CheckDeviceBeforeTest())
                {
                    case CacheService.DeviceCheckEnum.Ready:
                        Services.Cache.TestType = TestTypeEnum.Short;
                        await Services.DeviceService.Current.StartTest(BreathTestEnum.Start6Second);
                        await Services.Navigation.BreathManeuverFeedbackView();
                        break;
                    case CacheService.DeviceCheckEnum.DevicePurging:
                        await Services.Dialogs.ShowSecondsProgressAsync($"Device purging..", Services.DeviceService.Current.DeviceReadyCountDown);
                        break;
                    case CacheService.DeviceCheckEnum.HumidityOutOfRange:
                        Services.Dialogs.ShowAlert($"Humidity level ({Services.Cache.EnvironmentalInfo.Humidity}%) is out of range.", "Unable to Run Test", "Close");
                        break;
                    case CacheService.DeviceCheckEnum.PressureOutOfRange:
                        Services.Dialogs.ShowAlert($"Pressure level ({Services.Cache.EnvironmentalInfo.Pressure} kPa) is out of range.", "Unable to Run Test", "Close");
                        break;
                    case CacheService.DeviceCheckEnum.TemperatureOutOfRange:
                        Services.Dialogs.ShowAlert($"Temperature level ({Services.Cache.EnvironmentalInfo.Temperature} °C) is out of range.", "Unable to Run Test", "Close");
                        break;
                    case CacheService.DeviceCheckEnum.BatteryCriticallyLow:
                        Services.Dialogs.ShowAlert($"Battery Level ({Services.Cache.EnvironmentalInfo.BatteryLevel}%) is critically low.", "Unable to Run Test", "Close");
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
