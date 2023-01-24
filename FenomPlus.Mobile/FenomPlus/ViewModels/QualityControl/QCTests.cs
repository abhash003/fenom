using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace FenomPlus.ViewModels.QualityControl
{
    public class QCTest
    {
        public ObjectId Id { get; }

        public string DeviceSerialNumber { get; }

        // "User" or "Negative Control"
        public string UserName { get; }

        public DateTime TestDate { get; }

        public int TestResult { get; }

        public QCTest(string deviceSerialNumber, string userName, DateTime testDate, int testResult)
        {
            Id = ObjectId.NewObjectId();
            DeviceSerialNumber = deviceSerialNumber;
            UserName = userName;
            TestDate = testDate;
            TestResult = testResult;
        }

        [BsonCtor]
        public QCTest(ObjectId id, string deviceSerialNumber, string userName, DateTime testDate, int testResult)
        {
            Id = id;
            DeviceSerialNumber = deviceSerialNumber;
            UserName = userName;
            TestDate = testDate;
            TestResult = testResult;
        }
    }

}
