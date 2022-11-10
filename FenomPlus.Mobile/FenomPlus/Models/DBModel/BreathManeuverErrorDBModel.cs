using FenomPlus.Database.Tables;
using FenomPlus.Enums;
using FenomPlus.SDK.Core.Models;
using FenomPlus.Services;
using System;
using System.Globalization;
using Xamarin.Essentials;

namespace FenomPlus.Models
{
    public class BreathManeuverErrorDBModel : BreathManeuverErrorTb
    {

        public BreathManeuverErrorDBModel() : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static BreathManeuverErrorDBModel Create(BreathManeuver input)
        {
            int statusCode = (input.StatusCode >= ErrorCodesEnum.code.Length) ? ErrorCodesEnum.code.Length : input.StatusCode;

            // Get current version of software
            VersionTracking.Track();

            return new BreathManeuverErrorDBModel()
            {
                ErrorCode = ErrorCodesEnum.code[statusCode],
                Description = ErrorCodesEnum.title[statusCode],
                SerialNumber = IOC.Services.Cache.DeviceSerialNumber,
                Software = VersionTracking.CurrentVersion,
                Firmware = IOC.Services.Cache.Firmware,
                DateError = DateTime.Now.ToString(Constants.DateTimeFormatString, CultureInfo.CurrentCulture),
                Humidity = IOC.Services.Cache.EnvironmentalInfo.Humidity.ToString()
            };
        }
    }
}
 