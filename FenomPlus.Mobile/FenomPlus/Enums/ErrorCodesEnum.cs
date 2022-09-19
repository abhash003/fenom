using System;

namespace FenomPlus.Enums
{
    public class ErrorCodesEnum
    {
        public static string[] codes = new string[] {
            "Test performed OK",                                        // 0x00
            "Error: low battery",                                       // 0x01
            "Error: breath flow rate > 4.4 LPM anytime during T2",      // 0x02
            "Error: breath flow rate < 1 LPM anytime during T2",        // 0x03
            "Error: breath flow rate outside [2.5; 3.5] LPM for > 3 seconds during washout",    // 0x04
            "Error: breath flow rate outside[2.7; 3.3] LPM for > 3 seconds after washout",      // 0x05
            "Calculating test results",                                 // 0x06
            "Error: temperature too high(ambient temperature > 40 C)",  // 0x07
            "Error: temperature too low(ambient temperature < 15 C)",   // 0x08
            "Error: too dry(humidity< 20% relative humidity)",          // 0x09
            "Error: too humid(humidity > 90% relative humidity)",       // 0x0A
            "Error: pressure low(atmospheric pressure < 760 hPa)",      // 0x0B
            "Error: pressure high(atmospheric pressure > 1100 hPa)",    // 0x0C
            "Flow Unsteady[2.5; 3.5 ->2.7;3.3]",                        // 0x0D
            "No Score Detected",                                        // 0x0E
            "",                                                         // 0x0F
            "Breathing to long"                                         // 0x10
        };
    }
}
// 0x10