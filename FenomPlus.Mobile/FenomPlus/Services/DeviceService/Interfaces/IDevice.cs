#region Operational Defines

#define DS_FILTER_DEVICES_ON_ADVERTS

#endregion

using System;
using System.Threading.Tasks;
using FenomPlus.Helpers;
using FenomPlus.Models;
using FenomPlus.SDK.Core.Features;
using FenomPlus.SDK.Core.Models;
using FenomPlus.Services.DeviceService.Enums;

namespace FenomPlus.Services.DeviceService.Interfaces
{
    public interface IDevice
    {
        // Properties

        Guid Id { get; }

        string Name { get; }

        bool Connected { get; }

        object NativeDevice { get; }

        // Methods

        Task<bool> StartTest(BreathTestEnum breathTestEnum);

        Task<bool> StopTest();

        Task<bool> WRITEREQUEST(MESSAGE message, short idvar_size);

        Task<bool> DEVICEINFO();

        Task ConnectAsync();

        Task ConnectToKnownDeviceAsync(Guid id);

        Task DisconnectAsync();

        Task<bool> CALIBRATION(ID_SUB iD_SUB, double cal1, double cal2, double cal3);

        Task<bool> DATETIME(string date, string time);

        Task<bool> SERIALNUMBER(string SerailNumber);

        Task<bool> MESSAGE(MESSAGE message);

        Task<bool> DEBUGMSG();

        Task<bool> ENVIROMENTALINFO();

        Task<bool> BREATHTEST(BreathTestEnum breathTestEnum = BreathTestEnum.Start10Second);

        Task<bool> BREATHMANUEVER();

        /*
         * 
         * LEGACY TO BE REMOVED
         * 
         */

        int DeviceReadyCountDown { get; set; }
        bool ReadyForTest { get; set; }
        bool BreathTestInProgress { get; set; }

        //bool IsConnected(bool devicePowerOn = false);
        bool IsNotConnectedRedirect(bool devicePowerOn = false);       

        EnvironmentalInfo EnvironmentalInfo { get; set; }
        BreathManeuver BreathManeuver { get; set; }
        DeviceInfo DeviceInfo { get; set; }
        DebugMsg DebugMsg { get; set; }
        DeviceStatusInfo DeviceStatusInfo { get; set; }
        ErrorStatusInfo ErrorStatusInfo { get; set; }

        public DeviceCheckEnum CheckDeviceBeforeTest();

        EnvironmentalInfo DecodeEnvironmentalInfo(byte[] data);
        BreathManeuver DecodeBreathManeuver(byte[] data);
        DeviceInfo DecodeDeviceInfo(byte[] data);
        DebugMsg DecodeDebugMsg(byte[] data);
        ErrorStatusInfo DecodeErrorStatusInfo(byte[] data);
        DeviceStatusInfo DecodeDeviceStatusInfo(byte[] data);

        string DeviceConnectedStatus { get; set; }
        string DeviceSerialNumber { get; set; }
        string Firmware { get; set; }
        bool FenomReady { get; set; }

        RangeObservableCollection<DebugLog> DebugList { get; set; }

        DateTime SensorExpireDate { get; set; }

        int NOScore { get; set; }

        float BreathFlow { get; set; }

        int FenomValue { get; set; }

        event EventHandler BreathFlowChanged;
    }
}
