using FenomPlus.Models;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace FenomPlus.ViewModels.QualityControl.Models
{
    public class QCUser
    {
        // Unique User Names stored in Users Table
        public const string DeviceName = "Device";
        public const string NegativeControlName = "Negative Control";

        // Status for Device...

        //•	Pass
        //    - Negative Control status is “Pass” and
        //    - QC User status is “Conditionally Qualified” or “Qualified”.
        //    - Both tests were done in the last 24 hours.

        //•	Fail
        //    - Negative Control status is “Fail” or
        //    - QC User status is “Disqualified”.
        //o Both tests were done in the last 24 hours.

        //•	Expired
        //    - Negative Control status is “Pass” and
        //    - QC User Qualification or Validity period is “Expired”.
        //o No tests have been done in the last 24 hours.

        //•	Insufficient Data
        //    - Negative Control status is “None”, test is required or
        //    - QC User test is “None”, test is required.

        public const string DevicePass = "Pass";
        public const string DeviceFail = "Fail";
        public const string DeviceExpired = "Expired";
        public const string DeviceInsufficientData = "Insufficient Data";


        // Status for Negative Control...

        //•	Pass
        //    - Negative Control value of ≤5 ppb.
        //•	Fail
        //    - Negative Control value of >5 ppb.
        //•	None
        //    - Negative Control test is required.

        public const string NegativeControlPass = "Pass";
        public const string NegativeControlFail = "Fail";
        public const string NegativeControlNone = "None";


        // Status for User...

        //•	Conditionally Qualified
        //     - Fewer than 4 tests have been performed by the QC User.
        //     - All tests within the Qualification Period are “Pass”.

        //•	Qualified
        //     - Latest test is within the expected range for the QC User.

        //•	Disqualified
        //      - Latest test is outside the expected range for the QC User.

        //•	None
        //      - QC User test is required.

        public const string UserConditionallyQualified = "Conditionally Qualified";
        public const string UserQualified = "Qualified";
        public const string UserDisqualified = "Disqualified";
        public const string UserNone = "None";

        public ObjectId Id { get; set; }

        public string DeviceSerialNumber { get; set; } = string.Empty;

        public string UserName { get; set; }  // User name

        public string CurrentStatus { get; set; }

        public DateTime DateCreated { get; set; }  // Date created

        public DateTime ExpiresDate { get; set; }  // Date in which this expires?

        public DateTime NextTestDate { get; set; }  // Next Test date?

        public string Explanation { get; set; }

        public List<double> ChartData { get; set; } = new List<double>();  // Next Test date?

        public QCUser()
        {
            Id = ObjectId.NewObjectId();
            DeviceSerialNumber = string.Empty;
            UserName = string.Empty;
            CurrentStatus = string.Empty;
            DateCreated = DateTime.MinValue;
            ExpiresDate = DateTime.MinValue;
            NextTestDate = DateTime.MinValue;
            ChartData = new List<double>();
        }

        public QCUser(string deviceSerialNumber, string userName, string status, DateTime createDate, DateTime expiresDate, DateTime nextDate, List<double> chartData)
        {
            Id = ObjectId.NewObjectId();
            DeviceSerialNumber = deviceSerialNumber;
            UserName = userName;
            CurrentStatus = status;
            DateCreated = createDate;
            ExpiresDate = expiresDate;
            NextTestDate = nextDate;
            ChartData = chartData;
        }

        [BsonCtor]
        public QCUser(ObjectId id, string deviceSerialNumber, string userName, string status, DateTime createDate, DateTime expiresDate, DateTime nextDate, List<double> chartData)
        {
            Id = id;
            DeviceSerialNumber = deviceSerialNumber;
            UserName = userName;
            CurrentStatus = status;
            DateCreated = createDate;
            ExpiresDate = expiresDate;
            NextTestDate = nextDate;
            ChartData = chartData;
        }
    }
}
