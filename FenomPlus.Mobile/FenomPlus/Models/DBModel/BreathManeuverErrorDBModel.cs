using FenomPlus.Database.Tables;
using FenomPlus.Enums;
using FenomPlus.Enums.ErrorCodes;
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
            int statusCode = input.StatusCode;

            // Get current version of software
            VersionTracking.Track();

            var db = new BreathManeuverErrorDBModel();
            db.ErrorCode = ErrorCodeLookup.Lookup(statusCode).Code;
            db.Description = ErrorCodeLookup.Lookup(statusCode).Message;
            db.SerialNumber = IOC.Services.Cache.DeviceSerialNumber;
            db.Software = VersionTracking.CurrentVersion;
            db.Firmware = IOC.Services.Cache.Firmware;
            db.DateError = DateTime.Now.ToString(Constants.DateTimeFormatString, CultureInfo.CurrentCulture);
            db.Humidity = IOC.Services.Cache.EnvironmentalInfo.Humidity.ToString();

            //return new BreathManeuverErrorDBModel()
            //{
            //    ErrorCode = ErrorCodesEnum.code[statusCode],
            //    Description = ErrorCodesEnum.title[statusCode],
            //    SerialNumber = IOC.Services.Cache.DeviceSerialNumber,
            //    Software = VersionTracking.CurrentVersion,
            //    Firmware = IOC.Services.Cache.Firmware,
            //    DateError = DateTime.Now.ToString(Constants.DateTimeFormatString, CultureInfo.CurrentCulture),
            //    Humidity = IOC.Services.Cache.EnvironmentalInfo.Humidity.ToString()
            //};

            return db;
        }
    }
}
 