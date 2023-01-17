
using System;
using System.Collections.Generic;

namespace FenomPlus.Database.Tables
{
    public class QualityControlTb : BaseTb<QualityControlTb>
    {
        public string SerialNumber { get; set; }
        public string QCExpiration { get; set; }
        public string DateTaken { get; set; }
        public string User { get; set; }
        public string QCStatus { get; set; }
        public double TestResult { get; set; }
    }

    public class QualityControlUsersTb : BaseTb<QualityControlUsersTb>
    {
        public string DateAdded { get; set; }

        public string UserName { get; set; }

        public string QCStatus { get; set; }


        public List<QCTestResults> TestResults = new List<QCTestResults>();

        public QCTestResults LastResult()
        {
            if (TestResults.Count <= 0)
                return null;

            return TestResults[TestResults.Count - 1];
        }

        public List<QCTestResults> LastFourResults()
        {
            if (TestResults.Count <= 0)
                return null;

            List<QCTestResults> results = new List<QCTestResults>();

            for (int i = TestResults.Count - 1; i >= 0; i--)
            {
                results.Add(TestResults[i]);
            }

            return results;
        }
    }

    public class QCTestResults
    {
        public DateTime TestDate { get; set; }

        public string DeviceSerialNumber { get; set; }

        public string TestType { get; set; }

        public string TestValue { get; set; }

        public string TestResult { get; set; }

    }

    public class QualityControlDevicesTb : BaseTb<QualityControlDevicesTb>
    {
        public string SerialNumber { get; set; }
        public string LastConnected { get; set; }
    }
}