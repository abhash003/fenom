using System;

namespace FenomPlus.SDK.Core
{
    public class Constants
    {
        public const string FenomService                    = "0000fe40-cc7a-482a-984a-7f2ed5b3e58f"; // bluetooth service
        public const string DeviceInfoCharacteristic        = "0000fe41-8e22-4541-9d4c-21edae82ed19"; // notify
        public const string EnvironmentalInfoCharacteristic = "0000fe43-8e22-4541-9d4c-21edae82ed19"; // notify
        public const string DeviceStatusCharacteristic      = "0000fe45-8e22-4541-9d4c-21edae82ed19";
        public const string BreathManeuverCharacteristic    = "0000fe47-8e22-4541-9d4c-21edae82ed19"; // notify
        public const string ErrorStatusCharacteristic       = "0000fe49-8e22-4541-9d4c-21edae82ed19";
        public const string DebugMessageCharacteristic      = "0000fe4b-8e22-4541-9d4c-21edae82ed19"; // notify
        public const string FeatureWriteCharacteristic      = "0000fe4d-8e22-4541-9d4c-21edae82ed19"; // start test
        public const string UnknownCharacteristic           = "0000fe4f-8e22-4541-9d4c-21edae82ed19";
    }
}
