
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using FenomPlus.Enums;
using FenomPlus.Enums.ErrorCodes;
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
        public bool TestConductable
        {
            get {
                /*
                if (Services.DeviceService.Current != null && Services.DeviceService.Current.IsNotConnectedRedirect())
                {
                    DeviceCheckEnum dc = Services.DeviceService.Current.CheckDeviceBeforeTest();
                    return (dc == DeviceCheckEnum.Ready || dc == DeviceCheckEnum.DevicePurging);
                }
                return false;
                */
                return true;
            }
        }  

        [RelayCommand]
        private async Task StartStandardTest()
        {
            if (Services.DeviceService.Current != null && Services.DeviceService.Current.IsNotConnectedRedirect())
            {
                //if (Services.Config.RunRequiresQC && QualityControl is expired)
                //{
                //    // ToDo: Check QC Status
                //    Services.Dialogs.ShowAlert($"Quality Control for this device is expired.", "QC Expired", "Close");
                //    return;
                //}

                DeviceCheckEnum deviceStatus = Services.DeviceService.Current.CheckDeviceBeforeTest();

                switch (deviceStatus)
                {
                    case DeviceCheckEnum.Ready:
                        Services.Cache.TestType = TestTypeEnum.Standard;
                        await Services.DeviceService.Current.StartTest(BreathTestEnum.Start10Second);
                        await Services.Navigation.BreathManeuverFeedbackView();
                        break;
                    case DeviceCheckEnum.DevicePurging:
                        await Services.Dialogs.NotifyDevicePurgingAsync(Services.DeviceService.Current.DeviceReadyCountDown);
                        if (Services.Dialogs.PurgeCancelRequest)
                        {
                            return;
                        }
                        Services.Cache.TestType = TestTypeEnum.Standard;
                        await Services.DeviceService.Current.StartTest(BreathTestEnum.Start10Second);
                        await Services.Navigation.BreathManeuverFeedbackView();
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
                    case DeviceCheckEnum.NoSensorMissing:
                        Services.Dialogs.ShowAlert($"Nitrous Oxide Sensor is missing.  Install a F150 sensor.", "Sensor Error", "Close");
                        break;
                    case DeviceCheckEnum.NoSensorCommunicationFailed:
                        Services.Dialogs.ShowAlert($"Nitrous Oxide Sensor communication failed.", "Sensor Error", "Close");
                        break;
                    case DeviceCheckEnum.Unknown:
                        var error = ErrorCodeLookup.Lookup(Services.DeviceService.Current.ErrorStatusInfo.ErrorCode);
                        Services.Dialogs.ShowAlert(((error != null) ? error.Message : "Unknown error"), "Unknown Error", "Close");
                        break;
                    case DeviceCheckEnum.ERROR_SYSTEM_NEGATIVE_QC_FAILED:
                        Services.Cache.TestType = TestTypeEnum.Standard;
                        await Services.DeviceService.Current.StartTest(BreathTestEnum.Start10Second);
                        await Services.Navigation.BreathManeuverFeedbackView();
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
                //if (Services.Config.RunRequiresQC && QualityControl is expired)
                //{
                //    // ToDo: Check QC Status
                //    Services.Dialogs.ShowAlert($"Quality Control for this device is expired.", "QC Expired", "Close");
                //    return;
                //}

                switch (Services.DeviceService.Current.CheckDeviceBeforeTest())
                {
                    case DeviceCheckEnum.Ready:
                        Services.Cache.TestType = TestTypeEnum.Short;
                        await Services.DeviceService.Current.StartTest(BreathTestEnum.Start6Second);
                        await Services.Navigation.BreathManeuverFeedbackView();
                        break;
                    case DeviceCheckEnum.DevicePurging:
                        await Services.Dialogs.NotifyDevicePurgingAsync(Services.DeviceService.Current.DeviceReadyCountDown);
                        if (Services.Dialogs.PurgeCancelRequest)
                        {
                            return;
                        }


                        Services.Cache.TestType = TestTypeEnum.Short;
                        await Services.DeviceService.Current.StartTest(BreathTestEnum.Start6Second);
                        await Services.Navigation.BreathManeuverFeedbackView();
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
                    case DeviceCheckEnum.NoSensorMissing:
                        Services.Dialogs.ShowAlert($"Nitrous Oxide Sensor is missing.  Install a F150 sensor.", "Sensor Error", "Close");
                        break;
                    case DeviceCheckEnum.NoSensorCommunicationFailed:
                        Services.Dialogs.ShowAlert($"Nitrous Oxide Sensor communication failed.", "Sensor Error", "Close");
                        break;
                    case DeviceCheckEnum.Unknown:
                        var error = ErrorCodeLookup.Lookup(Services.DeviceService.Current.ErrorStatusInfo.ErrorCode);
                        Services.Dialogs.ShowAlert(((error != null) ? error.Message : "Unknown error"), "Unknown Error", "Close");
                        break;
                    case DeviceCheckEnum.ERROR_SYSTEM_NEGATIVE_QC_FAILED:
                        Services.Cache.TestType = TestTypeEnum.Short;
                        await Services.DeviceService.Current.StartTest(BreathTestEnum.Start6Second);
                        await Services.Navigation.BreathManeuverFeedbackView();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
