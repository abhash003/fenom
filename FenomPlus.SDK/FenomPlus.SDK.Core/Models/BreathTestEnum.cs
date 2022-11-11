using System;

namespace FenomPlus.SDK.Core.Models
{
    public enum BreathTestEnum
    {
        Stop = 0x00,
        Start6Second = 0x01,
        Start10Second = 0x02,
        Start0ppb = 0x03,
        Training = 0x04,
        QualityControl = 0x05
    }
}
