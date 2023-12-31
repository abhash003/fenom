﻿using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FenomPlus.ViewModels.QualityControl.Models
{
    public class QCTest
    {
        public const string TestPass = "Pass";
        public const string TestFail = "Fail";

        public ObjectId Id { get; set; }

        public string DeviceSerialNumber { get; set; }

        // "User Name" or "Negative Control" or "Device"
        public string UserName { get; set; }

        public DateTime TestDate { get; set; } = DateTime.MinValue;

        public float? TestValue { get; set; }

        public string TestStatus { get; set; } // Pass or Fail
        public string TestType { get; set; } = "+"; // + : Positive test, - : Negative test

        public string TestTypeDetail => TestType == "+" ? "Positive" : "Negative";
        public string Explanation { get; set; } = string.Empty;

        public string QcImage { get; set; }

        public QCTest() { }
        public QCTest(string deviceSerialNumber, string userName, DateTime testDate, float? testValue, string testStatus, string image, string explanation = "", string testType = "+")
        {
            Id = ObjectId.NewObjectId();
            DeviceSerialNumber = deviceSerialNumber;
            UserName = userName;
            TestDate = testDate;
            TestValue = testValue;
            TestStatus = testStatus;
            TestType = testType;
            QcImage = image;
        }

        //[BsonCtor]
        //public QCTest(ObjectId id, string deviceSerialNumber, string userName, DateTime testDate, int testValue, string testStatus, string explanation)
        //{
        //    Id = id;
        //    DeviceSerialNumber = deviceSerialNumber;
        //    UserName = userName;
        //    TestDate = testDate;
        //    TestValue = testValue;
        //    TestStatus = testStatus;
        //    Explanation = explanation;
        //}
    }

}
