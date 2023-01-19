using System;
using System.Collections.Generic;
using System.Text;

namespace FenomPlus.Database.Tables
{
    public class QCResultTable : BaseTb<QCResultTable>
    {
        public string UserName { get; set; }

        public string DeviceSerialNumber { get; set; }

        public DateTime TestDate { get; set; }

        public string TestType { get; set; }

        public double TestValue { get; set; }

        public string TestStatus { get; set; }

        public QCResultTable(string deviceSerialNumber, string testType, double testValue)
        {
            TestDate = DateTime.Now;
            DeviceSerialNumber = deviceSerialNumber;
            TestType = testType;
            TestValue = testValue;
        }

    }
}
