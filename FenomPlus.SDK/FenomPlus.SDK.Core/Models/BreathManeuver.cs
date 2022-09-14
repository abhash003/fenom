using System;
using System.Runtime.InteropServices;

namespace FenomPlus.SDK.Core.Models
{
    // 8 bytes old * 9 bytes new
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class BreathManeuver : BaseCharacteristic
    {
        public static int Min = 14;

        public short TestNumber;
        public byte  TimeRemaining;     // 0-done (??) FE-FenomReady FF-ready
        public byte  Temperature;
        public byte  Pressure;
        public short BreathFlow;
        public short NOScore;
        public byte  AnalysisTimeLeft;
        public byte  StatusCode;
        public byte  BreathGaugePressure;
        public short NOCounts;
        public byte  SampleMassFlow;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BreathManeuver Create(byte[] data)
        {
            BreathManeuver breathManeuver = new BreathManeuver();
            return breathManeuver.Decode(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public BreathManeuver Decode(byte[] data)
        {
            try
            {
                Data = data;
                if ((data != null) && (data.Length >= Min))
                {
                    TestNumber = (short)((int)(((int)Data[0]) * 256 + (int)Data[1]));
                    TimeRemaining = Data[2];
                    Temperature = Data[3];
                    Pressure = Data[4];
                    BreathFlow = (short)((int)(((int)Data[5]) * 256 + (int)Data[6]));
                    NOScore = (short)((int)(((int)Data[7]) * 256 + (int)Data[8]));
                    AnalysisTimeLeft = Data[9];
                    StatusCode = Data[10];
                    BreathGaugePressure = Data[11];
                    NOCounts = (short)((int)(((int)Data[12]) * 256 + (int)Data[13]));
                    SampleMassFlow = Data[14];
                }
            } finally { }
            return this;
        }

    }
}
