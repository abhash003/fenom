using System;
using System.Collections.Generic;
using System.Text;

namespace FenomPlus
{
    public static class Constants
    {
        public static string DateTimeFormatString = "O"; // Sortable e.g. “2016-12-20T10:34:37.3456”

        public static string PrettyDateFormatString = "MMMM d, yyyy";
        public static string PrettyTimeFormatString = "MMMM d, yyyy h:mm tt"; // e.g. “November 10, 2022 3:08 PM”
        public static string PrettyHoursFormatString = "h:mm:ss"; 

        public static int DeviceExpired = 0;
        public static int DeviceWarning60Days = 60;

        public static int SensorExpired = 0;
        public static int SensorWarning60Days = 60;

        public static int QualityControlExpired = 0;
        public static int QualityControlExpirationWarning = 1;

        public static double PressureLow75 = 75.35; 
        public static double PressureWarning78 = 78;
        public static double PressureWarning108 = 108;
        public static double PressureHigh110 = 110.65;

        public static double TemperatureLow14 = 14.6;
        public static double TemperatureHigh35 = 35.4;

        public static double HumidityLow18 = 18;
        public static double HumidityWarning25 = 25;
        public static double HumidityWarning85 = 85;
        public static double HumidityHigh92 = 92;

        public static int BatteryCritical3 = 6;
        public static int BatteryWarning20 = 20;
        public static int BatteryLevel50 = 50;
        public static int BatteryLevel75 = 75;

        public static byte NoSensorMissing = 112;
        public static byte NoSensorCommunicationFailed = 113;
    }
}
