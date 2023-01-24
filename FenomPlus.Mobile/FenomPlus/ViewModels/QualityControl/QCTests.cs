using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace FenomPlus.ViewModels.QualityControl
{
    public class QCTest
    {
        public ObjectId TestId { get; }

        public string DeviceSerialNumber { get; }

        // "User" or "Negative Control"
        public string UserName { get; }

        public DateTime TestDate { get; }

        public int TestResult { get; }

        public QCTest(string deviceSerialNumber, string userName, DateTime testDate, int testResult)
        {
            TestId = ObjectId.NewObjectId();
            DeviceSerialNumber = deviceSerialNumber;
            UserName = userName;
            TestDate = testDate;
            TestResult = testResult;
        }

        [BsonCtor]
        public QCTest(ObjectId testId, string deviceSerialNumber, string userName, DateTime testDate, int testResult)
        {
            TestId = testId;
            DeviceSerialNumber = deviceSerialNumber;
            UserName = userName;
            TestDate = testDate;
            TestResult = testResult;
        }
    }

}
