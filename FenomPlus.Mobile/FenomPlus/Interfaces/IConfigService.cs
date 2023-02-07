using System;

namespace FenomPlus.Interfaces
{
    public interface IConfigService
    {
        int RingBufferSample { get; set; }
        int RingBufferTimeout { get; set; }
        int BreathFlowTimeout { get; set; }
        float GaugeDataLow { get; set; }
        float GaugeDataHigh { get; set; }
        int BatteryLevelLow { get; set; }
        int DaysRemaining { get; set; }
        int TestResultReadyWait { get; set; }
        int StopExhalingReadyWait { get; set; }
        bool RunRequiresQC { get; set; }
    }
}
