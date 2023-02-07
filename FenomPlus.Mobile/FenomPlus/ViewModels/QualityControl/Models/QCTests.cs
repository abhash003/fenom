using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace FenomPlus.ViewModels.QualityControl.Models
{
    public class QCTest
    {
        public ObjectId Id { get; set; }

        public string DeviceSerialNumber { get; set; }

        // "User Name" or "Negative Control" or "Device"
        public string UserName { get; set; }

        public DateTime TestDate { get; set; }

        public int TestResult { get; set; }

        public string Explanation { get; set; }

        public QCTest(string deviceSerialNumber, string userName, DateTime testDate, int testResult)
        {
            Id = ObjectId.NewObjectId();
            DeviceSerialNumber = deviceSerialNumber;
            UserName = userName;
            TestDate = testDate;
            TestResult = testResult;
            Explanation = string.Empty;
        }

        [BsonCtor]
        public QCTest(ObjectId id, string deviceSerialNumber, string userName, DateTime testDate, int testResult, string explanation)
        {
            Id = id;
            DeviceSerialNumber = deviceSerialNumber;
            UserName = userName;
            TestDate = testDate;
            TestResult = testResult;
            Explanation = explanation;
        }
    }

}
