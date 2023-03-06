using System;
using System.Globalization;
using FenomPlus.Database.Tables;
using FenomPlus.SDK.Core.Models;
using FenomPlus.Services;

namespace FenomPlus.Models
{
    public class BreathManeuverResultDBModel : BreathManeuverResultTb
    {
        public BreathManeuverResultDBModel() : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static BreathManeuverResultDBModel Create(BreathManeuver input, ErrorStatusInfo esi)
        {
            return new BreathManeuverResultDBModel()
            {
                BreathFlow = input.BreathFlow,
                DateOfTest = DateTime.Now.ToString(Constants.DateTimeFormatString, CultureInfo.CurrentCulture),
                NOScore = input.NOScore,
                StatusCode = esi.ErrorCode,
                TestNumber = input.TestNumber,
                
                SerialNumber = IOC.Services.DeviceService.Current?.DeviceSerialNumber,
                QCStatus = "?",
                TestType = IOC.Services.Cache.TestType.ToString(),
                TestResult = input.NOScore.ToString()
            };
        }
    }
}
