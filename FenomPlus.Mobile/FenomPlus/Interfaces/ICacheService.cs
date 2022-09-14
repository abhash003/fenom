using System;
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

        bool DeviceExpiring { get; set; }
        bool DeviceSensorExpiring { get; set; }
        bool BatteryStatus { get; set; }

        int BatteryLevel { get; set; }
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
        EnvironmentalInfo _EnvironmentalInfo { get; set; }
        BreathManeuver _BreathManeuver { get; set; }
        DeviceInfo _DeviceInfo { get; set; }
        DebugMsg _DebugMsg { get; set; }

        EnvironmentalInfo DecodeEnvironmentalInfo(byte[] data);
        BreathManeuver DecodeBreathManeuver(byte[] data);
        DeviceInfo DecodeDeviceInfo(byte[] data);
        DebugMsg DecodeDebugMsg(byte[] data);

        bool ReadyForTest { get; set; }
        bool FenomReady { get; set; }
        float FenomValue { get; set; }
    }
}
