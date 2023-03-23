using System.ComponentModel;
namespace FenomPlus.QualityControl.Enums
{
    public enum NegativeControlStatus // 2 bit
    {
        [Description("Negative Control test is required")]
        None = 0,
        [Description("Greater or Equal than 5 ppb on Negative Control Value")]
        Fail,
        [Description("Less Than 5 ppb on Negative Control Value")]
        Pass,
    }
    public enum UserStatus // 2 bit
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

    public enum ElapsedHrSinceLastPassingTest // 1 bit
    {
        [Description("Less Than 24 hr elapsed since last passing test ")]
        LT_24 = 0,
        [Description("Greater or Equal 24 hr elapsed since last passing test ")]
        GE_24,
    }

    public enum PeriodStatus // 3 bit
    {
        [Description("n/a")]
        None = 0,

        [Description("Greater or Equal than 24 hr elapsed since last passing test by a Qualified User")]
        ExpiredValidity = UserStatus.Qualified << 1 | ElapsedHrSinceLastPassingTest.GE_24,

        [Description("Greater or Equal than 24 hr elapsed since last passing test by a Conditionally Qualified User")]
        ExpiredUserQualification = UserStatus.ConditionallyQualified << 1 | ElapsedHrSinceLastPassingTest.GE_24,

        [Description("Less Than 24 hr elapsed since last passing test by a Qualified User")]
        WithinValidity = UserStatus.Qualified << 1 | ElapsedHrSinceLastPassingTest.LT_24,

        [Description("Less Than 24 hr elapsed since last passing test by a Conditionally Qualified User")]
        WithinUserQualification = UserStatus.ConditionallyQualified << 1 | ElapsedHrSinceLastPassingTest.LT_24
    }
    public enum Last24HrTestCount
    {
        [Description("No test performed in the last 24 hr")]
        Zero = 0,
        [Description("Both tests performed the last 24 hr")]
        Both = 2
    }

    public enum DeviceStatus
    {
        [Description("NegativeControlStatus.None || UserStatus.None i.e. test is required")]
        InsufficientData = 0,

        [Description("NegativeControlStatus.Pass && ( UserStatus.Qualified || PeriodStatus.ExpiredValidity ) " +
            "&& No test performed in the last 24 hr")]
        Expired = 1,

        [Description("( NegativeControlStatus.Fail || UserStatus.Disqualified ) " +
            "&& Both tests performed in the last 24 hr")]
        Fail = 2,

        [Description("NegativeControlStatus.Pass && (UserStatus.ConditionalyQualified || UserStatus.Qualified) " +
            "&& Both tests performed in the last 24 hr")]
        Pass = 3,
    }
}


