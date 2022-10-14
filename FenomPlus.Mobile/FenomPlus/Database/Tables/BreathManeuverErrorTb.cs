using System;
using System.Collections.Generic;

namespace FenomPlus.Database.Tables
{
    public class BreathManeuverErrorTb : BaseTb<BreathManeuverErrorTb>
    {
        public string ErrorCode { get; set; }
        public string Description { get; set; }
        public string Humidity { get; set; }
        public string DateError { get; set; }
        public string SerialNumber { get; set; }
        public string Firmware { get; set; }
        public string Software { get; set; }
    }
}