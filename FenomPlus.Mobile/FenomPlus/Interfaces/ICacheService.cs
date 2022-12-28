using System;
using FenomPlus.Enums;
using FenomPlus.Helpers;
using FenomPlus.Models;
using FenomPlus.SDK.Core.Models;
using Microsoft.Extensions.Logging;

namespace FenomPlus.Interfaces
{
    public interface ICacheService
    {
        ILoggerFactory Logger { get; set; }
        string QCUsername { get; set; }

        DateTime DeviceExpireDate { get; set; }
        DateTime SensorExpireDate { get; set; }
        string DeviceConnectedStatus { get; set; }
        string DeviceSerialNumber { get; set; }
        string Firmware { get; set; }
        TestTypeEnum TestType { get; set; }
        float BreathFlow { get; set; }
        int BreathFlowTimer { get; set; }
        int NOScore { get; set; }

        float HumanControlResult { get; set; }

        RangeObservableCollection<DebugLog> DebugList { get; set; }
        EnvironmentalInfo EnvironmentalInfo { get; set; }
        BreathManeuver BreathManeuver { get; set; }
        DeviceInfo DeviceInfo { get; set; }
        DebugMsg DebugMsg { get; set; }

        EnvironmentalInfo DecodeEnvironmentalInfo(byte[] data);
        BreathManeuver DecodeBreathManeuver(byte[] data);
        DeviceInfo DecodeDeviceInfo(byte[] data);
        DebugMsg DecodeDebugMsg(byte[] data);
        ErrorStatusInfo DecodeErrorStatusInfo(byte[] data);
        DeviceStatusInfo DecodeDeviceStatusInfo(byte[] data);

        event EventHandler BreathFlowChanged;

        bool ReadyForTest { get; set; }
        bool FenomReady { get; set; }
        float FenomValue { get; set; }

        public bool CheckDeviceBeforeTest();
    }
}
