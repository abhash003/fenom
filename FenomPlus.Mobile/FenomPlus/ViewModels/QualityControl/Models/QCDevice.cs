using LiteDB;
using System;
using FenomPlus.Services;
using FenomPlus.Interfaces;

namespace FenomPlus.ViewModels.QualityControl.Models
{
    public class QCDevice
    {
        // Status for Device...

        //• Valid	
        //    - Negative Control status is “Pass” and
        //    - QC User status is “Conditionally Qualified” or “Qualified”.
        //    - Both tests were done in the last 24 hours.

        //•	Failed
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

        public const string DeviceValid = "Valid";
        public const string DeviceFail = "Fail";
        public const string DeviceExpired = "Expired";
        public const string DeviceInsufficientData = "Insufficient Data";

        // public IAppServices Services => IOC.Services;

        public ObjectId Id { get; set; }

        public string DeviceSerialNumber { get; set; }

        public string CurrentStatus { get; set; }

        public bool RequireQC { get; set; }

        public DateTime DateCreated { get; set; }  // Date created

        public DateTime DateUpdated { get; set; }  // Date created

        public QCDevice(string deviceSerialNumber)
        {
            Id = ObjectId.NewObjectId();
            DeviceSerialNumber = deviceSerialNumber;
            CurrentStatus = DeviceExpired;
            RequireQC = (IOC.Services.DeviceService.Current != null) ? IOC.Services.DeviceService.Current.IsQCEnabled() : false;
            DateCreated = DateTime.Now;
            DateUpdated = DateTime.Now; 
        }
    }
}
