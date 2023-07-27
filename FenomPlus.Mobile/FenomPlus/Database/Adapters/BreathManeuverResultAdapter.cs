using System;
using System.Diagnostics;
using System.Globalization;
using FenomPlus.Database.Tables;
using FenomPlus.Models;

namespace FenomPlus.Database.Adapters
{
    public static class BreathManeuverResultAdapter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static BreathManeuverResultDBModel Convert(this BreathManeuverResultTb input)
        {
            if (input == null) return null;
            return new BreathManeuverResultDBModel()
            {
                _id = input._id,
                BreathFlow = input.BreathFlow,
                DateOfTest = input.DateOfTest,
                NOScore = input.NOScore,
                StatusCode = input.StatusCode,
                TestNumber = input.TestNumber,
                QCStatus = input.QCStatus,
                SerialNumber = input.SerialNumber,
                TestResult = input.TestResult,
                TestType = input.TestType
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static BreathManeuverResultTb Convert(this BreathManeuverResultDBModel input)
        {
            if (input == null) return null;
            return new BreathManeuverResultTb()
            {
                _id = input._id,
                BreathFlow = input.BreathFlow,
                DateOfTest = input.DateOfTest,
                NOScore = input.NOScore,
                StatusCode = input.StatusCode,
                TestNumber = input.TestNumber,
                QCStatus = input.QCStatus,
                SerialNumber = input.SerialNumber,
                TestResult = input.TestResult,
                TestType = input.TestType
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static BreathManeuverResultDataModel ConvertForGrid(this BreathManeuverResultTb input)
        {
            if (input == null) return null;

            string prettyDateTime;

            if (DateTime.TryParse(input.DateOfTest, out var dt))
            {
                prettyDateTime = dt.ToString(Constants.PrettyTimeFormatString, CultureInfo.CurrentCulture);
            }
            else
            {
                prettyDateTime = input.DateOfTest;
            }

            if (input.QCStatus == "Valid")
            {
                input.QCStatus = "QualityControlFull.png";
            }
            else if (input.QCStatus == "Expired" || input.QCStatus == "Fail")
            {
                input.QCStatus = "quality_control_red.png";
            }
            else if (input.QCStatus == "Disabled")
            {
                input.QCStatus = "QualityControl.png";
            }
            else
            {
                input.QCStatus = "QualityControlWarning.png";
            }

            Debug.WriteLine($"TestResult = {input.TestResult}");

            return new BreathManeuverResultDataModel()
            { _id = input._id,
                BreathFlow = input.BreathFlow,
                DateOfTest = prettyDateTime,
                NOScore = input.NOScore,
                StatusCode = input.StatusCode,
                TestNumber = input.TestNumber,
                QCStatus = input.QCStatus,
                SerialNumber = input.SerialNumber,
                TestResult = input.TestResult,
                TestType = input.TestType
            };
        }
    }
}
