using System;

namespace FenomPlus.Enums
{
    public class ErrorCodesEnum
    {
        public static string[] codes = new string[] {
            "Test performed OK",
            "Error: low battery",
            "Error: breath flow rate > 4.4 LPM anytime during T2",
            "Error: breath flow rate < 1 LPM anytime during T2",
            "Error: breath flow rate outside [2.5; 3.5] LPM for > 3 seconds during washout",
            "Error: breath flow rate outside[2.7; 3.3] LPM for > 3 seconds after washout",
            "Calculating test results",
            "Error: temperature too high(ambient temperature > 40 C)",
            "Error: temperature too low(ambient temperature < 15 C)",
            "Error: too dry(humidity< 20% relative humidity)",
            "Error: too humid(humidity > 90% relative humidity)",
            "Error: pressure low(atmospheric pressure < 760 hPa)",
            "Error: pressure high(atmospheric pressure > 1100 hPa)",
            "Flow Unsteady[2.5; 3.5 ->2.7;3.3]",
            "No Score Detected"
        };
    }
}
