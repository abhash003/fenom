using FenomPlus.Models;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;
using Syncfusion.XlsIO.Parser.Biff_Records.Charts;

namespace FenomPlus.ViewModels.QualityControl.Models
{
    public class QCUser
    {
        public const string NegativeControlName = "Negative Control";

        // Status for Negative Control...

        //•	Pass
        //    - Negative Control value of ≤5 ppb.
        //•	Fail
        //    - Negative Control value of >5 ppb.
        //•	None
        //    - Negative Control test is required.

        public const string NegativeControlPass = "Pass";
        public const string NegativeControlFail = "Fail";
        public const string NegativeControlExpired = "Expired";
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

        public string DeviceSerialNumber { get; set; }

        public string UserName { get; set; }  // User name

        public string CurrentStatus { get; set; }

        public DateTime DateCreated { get; set; }  // Date created

        public DateTime ExpiresDate { get; set; }  // Date in which this expires?

        public DateTime NextTestDate { get; set; }  // Next Test date?

        public string Explanation { get; set; }

        public bool  ShowChartOption { get; set; }

        public int C1 { get; set; }
        public DateTime C1Date{ get; set; }

        public int C2 { get; set; }
        public DateTime C2Date { get; set; }

        public int C3 { get; set; }
        public DateTime C3Date { get; set; }

        public int QCT { get; set; }

        public QCUser(string deviceSerialNumber, string userName)
        {
            Id = ObjectId.NewObjectId();
            DeviceSerialNumber = deviceSerialNumber;
            UserName = userName;
            CurrentStatus = UserNone;
            DateCreated = DateTime.Now;
            ExpiresDate = DateTime.MinValue;
            NextTestDate = DateTime.MinValue;
            Explanation = string.Empty;
            ShowChartOption = false;
            C1 = 0;
            C1Date = DateTime.MinValue;
            C2 = 0;
            C2Date = DateTime.MinValue;
            C3 = 0;
            C3Date = DateTime.MinValue;
            QCT = 0;
        }
    }
}
