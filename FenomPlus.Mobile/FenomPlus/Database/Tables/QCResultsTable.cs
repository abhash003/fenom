using System;
using System.Collections.Generic;
using System.Text;

namespace FenomPlus.Database.Tables
{
    public class QCResultsTable : BaseTb<QCResultsTable>
    {
        public DateTime TestDate { get; set; }

        public string DeviceSerialNumber { get; set; }

        public string TestType { get; set; }

        public double TestValue { get; set; }

        public string TestResult { get; set; }

        public QCResultsTable(DateTime dateTime, string deviceSerialNumber, string testType, double testValue)
        {
            TestDate = dateTime;
            DeviceSerialNumber = deviceSerialNumber;
            TestType = testType;
            TestValue = testValue;
        }

    }
}
