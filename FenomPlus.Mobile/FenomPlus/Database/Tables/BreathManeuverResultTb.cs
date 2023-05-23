
namespace FenomPlus.Database.Tables
{
    public class BreathManeuverResultTb : BaseTb<BreathManeuverResultTb>
    {
        // from sensor all info here
        public int TestNumber { get; set; }
        public float BreathFlow { get; set; }
        public int?  NOScore { get; set; }
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

        public string TestDateTime =>
            //var dateInfo = DateOfTest.Split('T');
            //return $"{dateInfo[0]}  {dateInfo[1]}";
            DateOfTest.Replace("T", "   ");
        public string TestTypeDetail => TestType == "Standard" ?"Standard - 10s" :"Short - 6s";
    }
}