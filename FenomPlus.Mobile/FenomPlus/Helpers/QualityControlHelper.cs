﻿using FenomPlus.QualityControl.Enums;
namespace FenomPlus.Helpers
{
    internal class QualityControlHelper
    {
        public DeviceStatus GetDeviceStatus(NegativeControlStatus nc, UserStatus us, PeriodStatus ps, Last24HrTestCount lt)
        {
            if (nc == NegativeControlStatus.None || us == UserStatus.None)
            {
                return DeviceStatus.InsufficientData;
            }
            else if (nc == NegativeControlStatus.Pass &&
                (us == UserStatus.Qualified || ps == PeriodStatus.ExpiredValidity)
                && lt == Last24HrTestCount.Zero)
            {
                return DeviceStatus.Expired;
            }
            else if ((nc == NegativeControlStatus.Fail || us == UserStatus.Disqualified) && lt == Last24HrTestCount.Both)
            {
                return DeviceStatus.Fail;
            }
            else if (nc == NegativeControlStatus.Pass
                && (us == UserStatus.ConditionallyQualified || us == UserStatus.Qualified)
                && lt == Last24HrTestCount.Both)
            {
                return DeviceStatus.Pass;
            }
            else
            {
                return DeviceStatus.InsufficientData;
            }
        }
    }
}
