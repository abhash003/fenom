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

        Task<bool> ToggleQC(bool Enable = true);
        Task<bool> ExtendDeviceValidity(short hour);
        Task<bool> SendFailMsg(ushort val);

        bool IsQCEnabled(); // if qc is populated in last device info update
        bool GetQCHoursRemaining(ref short hour); // >=0 : valid, <=-1 : expired, = 0x8000 : failed
        bool ExtendQC(int hours);

        Task<bool> WRITEREQUEST(MESSAGE message, short idvar_size);

        Task<bool> DEVICEINFO();        

        Task ConnectAsync();

        Task ConnectToKnownDeviceAsync(Guid id);

        Task DisconnectAsync();

        Task<bool> CALIBRATION(ID_SUB iD_SUB, double cal1, double cal2, double cal3);

        Task<bool> DATETIME(string date, string time);

        Task<bool> SERIALNUMBER(string SerailNumber);

        Task<bool> WriteRequest(MESSAGE message, short sz = 1);

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
        byte LastStatusCode { get; set; }
        int LastErrorCode { get; set; }
        DeviceInfo DeviceInfo { get; set; }
        DebugMsg DebugMsg { get; set; }
        DeviceStatusInfo DeviceStatusInfo { get; set; }
        ErrorStatusInfo ErrorStatusInfo { get; set; }

        public DeviceCheckEnum CheckDeviceBeforeTest(bool isQCCheck = false);

        void DecodeEnvironmentalInfo(byte[] data);
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

        int? NOScore { get; set; }

        float BreathFlow { get; set; }

        float? FenomValue { get; set; }

        event EventHandler BreathFlowChanged;

        Task<bool> RequestDeviceInfo();
        Task<bool> RequestEnvironmentalInfo();
        string GetDeviceQCStatus();

        int? DeviceLifeRemaining { get; set; }
    }
}
