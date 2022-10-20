using System;
using FenomPlus.Interfaces;

namespace FenomPlus.Services
{
    public class ConfigService : BaseService, IConfigService
    {
        public int RingBufferSample { get; set; }
        public int RingBufferTimeout { get; set; }
        public int BreathFlowTimeout { get; set; }
        public float GaugeDataLow { get; set; }
        public float GaugeDataHigh { get; set; }
        public int BatteryLevelLow { get; set; }
        public int DaysRemaining { get; set; }
        public int TestResultReadyWait { get; set; }
        public int StopExhalingReadyWait { get; set; }

        public ConfigService(IAppServices services) : base(services)
        {
            RingBufferSample = 10;          // total samples
            RingBufferTimeout = 1000;       // mill-seconds
            BreathFlowTimeout = 50;         // mill-seconds
            GaugeDataLow = 2.8f;
            GaugeDataHigh = 3.2f;
            BatteryLevelLow = 10;           // %
            DaysRemaining = 60;             // days
            TestResultReadyWait = 22;       // seconds
            StopExhalingReadyWait = 3;      // seconds
        }
    }
}
