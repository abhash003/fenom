﻿using System;
using System.Globalization;
using FenomPlus.Database.Tables;
using FenomPlus.Models;

namespace FenomPlus.Database.Adapters
{
    public static class BreathManeuverErrorAdapter
    {
        public static BreathManeuverErrorDBModel Convert(this BreathManeuverErrorTb input)
        {
            if (input == null) return null;
            return new BreathManeuverErrorDBModel()
            {
                _id = input._id,
                DateError = input.DateError,
                Description = input.Description,
                Firmware = input.Firmware,
                Humidity = input.Humidity,
                SerialNumber = input.SerialNumber,
                Software = input.Software
            };
        }

        public static BreathManeuverErrorTb Convert(this BreathManeuverErrorDBModel input)
        {
            if (input == null) return null;
            return new BreathManeuverErrorTb()
            {
                _id = input._id,
                DateError = input.DateError,
                Description = input.Description,
                Firmware = input.Firmware,
                Humidity = input.Humidity,
                SerialNumber = input.SerialNumber,
                Software = input.Software
            };
        }

        public static BreathManeuverErrorDataModel ConvertForGrid(this BreathManeuverErrorTb input)
        {
            if (input == null) 
                return null;

            string prettyDateTime;

            if (DateTime.TryParse(input.DateError, out var dt))
            {
                prettyDateTime = dt.ToString(Constants.PrettyTimeFormatString, CultureInfo.CurrentCulture);
            }
            else
            {
                prettyDateTime = input.DateError;
            }

            return new BreathManeuverErrorDataModel()
            {
                _id = input._id,
                DateError = prettyDateTime,
                Description = input.Description,
                Firmware = input.Firmware,
                Humidity = input.Humidity,
                SerialNumber = input.SerialNumber,
                Software = input.Software
            };
        }
    }
}
