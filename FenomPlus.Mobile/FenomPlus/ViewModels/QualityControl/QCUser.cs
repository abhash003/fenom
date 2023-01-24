using FenomPlus.Models;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace FenomPlus.ViewModels.QualityControl
{
    public class QCUser
    {
        public ObjectId UserId { get; }

        public string DeviceSerialNumber { get; set; }

        public string UserName { get; set; }  // User name

        // Status...

        //•	Conditionally Qualified
        //     - Fewer than 4 tests have been performed by the QC User.
        //     - All tests within the Qualification Period are “Pass”.

        //•	Qualified
        //     - Latest test is within the expected range for the QC User.

        //•	Disqualified
        //      - Latest test is outside the expected range for the QC User.

        //•	None
        //      - QC User test is required.

        public string CurrentStatus { get; set; }

        public DateTime DateCreated { get; set; }  // Date created

        public DateTime ExpiresDate { get; set; }  // Date in which this expires?

        public DateTime NextTestDate { get; set; }  // Next Test date?

        public QCUser(string deviceSerialNumber, string userName, string status, DateTime createDate, DateTime expiresDate, DateTime nextDate)
        {
            UserId = ObjectId.NewObjectId();
            DeviceSerialNumber = deviceSerialNumber;
            UserName = userName;
            CurrentStatus = status;
            DateCreated = createDate;
            ExpiresDate = expiresDate;
            NextTestDate = nextDate;

        }

        [BsonCtor]
        public QCUser(ObjectId userId, string deviceSerialNumber, string userName, string status, DateTime createDate, DateTime expiresDate, DateTime nextDate)
        {
            UserId = userId;
            DeviceSerialNumber = deviceSerialNumber;
            UserName = userName;
            CurrentStatus = status;
            DateCreated = createDate;
            ExpiresDate = expiresDate;
            NextTestDate = nextDate;
        }
    }
}
