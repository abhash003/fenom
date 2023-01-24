using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace FenomPlus.ViewModels.QualityControl
{
    public class QCDevice
    {
        public ObjectId DeviceId { get; }

        public string DeviceSerialNumber { get; set; }

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

        public string DeviceStatus { get; set; }

        public QCDevice(string deviceSerialNumber, string deviceStatus)
        {
            DeviceId = ObjectId.NewObjectId();
            DeviceSerialNumber = deviceSerialNumber;
            DeviceStatus = deviceStatus;
        }

        [BsonCtor]
        public QCDevice(ObjectId testId, string deviceSerialNumber, string deviceStatus)
        {
            DeviceId = testId;
            DeviceSerialNumber = deviceSerialNumber;
            DeviceStatus = deviceStatus;
        }
    }
}
