using FenomPlus.Database.Tables;
using FenomPlus.Enums;
using FenomPlus.Enums.ErrorCodes;
using FenomPlus.SDK.Core.Models;
using FenomPlus.Services;
using Syncfusion.CompoundFile.Net;
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

            var error = ErrorCodeLookup.Lookup(statusCode);
            db.ErrorCode = error.Code;
            db.Description = error.Message;
            db.SerialNumber = IOC.Services.DeviceService.Current?.DeviceSerialNumber;

            db.Software = VersionTracking.CurrentVersion;
            db.Firmware = IOC.Services.DeviceService.Current?.Firmware;
            db.DateError = DateTime.Now.ToString(Constants.DateTimeFormatString, CultureInfo.CurrentCulture);
            db.Humidity = IOC.Services.DeviceService.Current?.EnvironmentalInfo.Humidity.ToString();

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
 