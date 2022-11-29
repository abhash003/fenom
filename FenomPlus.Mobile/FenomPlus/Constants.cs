using System;
using System.Collections.Generic;
using System.Text;

namespace FenomPlus
{
    public static class Constants
    {
        public static string DateTimeFormatString = "O"; // Sortable e.g. “2016-12-20T10:34:37.3456”
        public static string PrettyTimeFormatString = "MMMM d, yyyy h:mm tt"; // e.g. “November 10, 2022 3:08 PM”

        public static int BatteryCritical = 3;
        public static int BatteryWarning = 20;
        public static int Battery50 = 50;
        public static int Battery75 = 75;
        public static int Battery100 = 100;

        public static int SensorLow = 0;
        public static int SensorWarning = 60;

        public static int QualityControlExpirationLow = 0;
        public static int QualityControlExpirationWarning = 1;
        public static int QualityControlExpirationFull = 7;

        public static int DeviceLow = 0;
        public static int DeviceWarning = 60;
        public static int DeviceFull = 1 * 365 + 10;

        public static int RelativeHumidityLow = 18;
        public static int RelativeHumidityWarning = 92;

        public static int TemperatureLow = 15;
        public static int TemperatureWarning = 35;

        public static int PressureLow = 75;
        public static int PressureWarning = 110;
        //public static int PressureFull = 100;
    }
}
