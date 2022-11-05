﻿
namespace FenomPlus.Database.Tables
{
    public class BreathManeuverResultTb : BaseTb<BreathManeuverResultTb>
    {
        // from sensor all info here
        public int TestNumber { get; set; }
        public int Temperature { get; set; }
        public int Pressure { get; set; }
        public int BreathFlow { get; set; }
        public int NOScore { get; set; }
        public int StatusCode { get; set; }

        // for grid display
        public string SerialNumber { get; set; }
        public string TestType { get; set; }
        public string DateOfTest { get; set; }
        public string QCStatus { get; set; }
        public string TestResult { get; set; }

        public string TestDate
        {
            get
            {
                var dateInfo = DateOfTest.Split('T');
                return dateInfo[0].ToString();
            }
        }

        public string TestTime
        {
            get
            {
                var dateInfo = DateOfTest.Split('T');
                return dateInfo[1].ToString();
            }
        }

        public string TestDateTime
        {
            get
            {
                //var dateInfo = DateOfTest.Split('T');
                //return $"{dateInfo[0]}  {dateInfo[1]}";
                return DateOfTest.Replace("T", "   ");
            }
        }

    }
}