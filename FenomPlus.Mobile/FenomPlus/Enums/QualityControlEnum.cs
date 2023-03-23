using System.ComponentModel;
namespace FenomPlus.QualityControl.Enums
{
    public enum NegativeControlStatus
    {
        [Description("Negative Control test is required")]
        None,
        [Description("Greater or Equal than 5 ppb on Negative Control Value")]
        Fail,
        [Description("Less Than 5 ppb on Negative Control Value")]
        Pass,
    }
    public enum UserStatus
    {
        [Description("QC User test is required")]
        None = 0,
        [Description("Latest test is out of the expected range for the QC User")]
        Disqualified = 1,

        [Description("Less Than 4 tests have been performed by the QC User, all tests within the Qualification Period are 'Pass'")]
        ConditionallyQualified = 2,

        [Description("Latest test is within the expected range for the QC User")]
        Qualified = 3,
    }

    public enum PeriodStatus
    {
        [Description("n/a")]
        None,

        [Description("Greater or Equal than 24 hr elapsed since last passing test by a Qaulified User")]
        ExpiredValidity,

        [Description("Greater or Equal than 24 hr elapsed since last passing test by a Conditionally Qaulified User")]
        ExpiredUserQualification,

        [Description("Less Than 24 hr elapsed since last passing test by a Qaulified User")]
        WithinValidity,

        [Description("Less Than 24 hr elapsed since last passing test by a Conditionally Qaulified User")]
        WithinUserQualification,
    }
    public enum Last24HrTestStatus
    {
        [Description("No test performed in the last 24 hr")]
        Zero = 0,
        [Description("Both tests performed the last 24 hr")]
        Both = 2
    }

    public enum DeviceStatus
    {
        [Description("NegativeControlStatus.None || UserStatus.None i.e. test is required")]
        InsufficientData = -3,

        [Description("NegativeControlStatus.Pass && ( UserStatus.Qualified || PeriodStatus.ExpiredValidity ) " +
            "&& No test performed in the last 24 hr")]
        Expired = -2,

        [Description("( NegativeControlStatus.Fail || UserStatus.Disqualified ) " +
            "&& Both tests performed in the last 24 hr")]
        Fail = -1,

        [Description("NegativeControlStatus.Pass && (UserStatus.ConditionalyQualified || UserStatus.Qualified) " +
            "&& Both tests performed in the last 24 hr")]
        Pass = 1,
    }
}


