using FenomPlus.Services.DeviceService.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;

namespace FenomPlus.Enums
{
    namespace ErrorCodes
    {
        public class ErrorCodeLookup
        {
            protected static Dictionary<int, string> _errorCodes;

            public ErrorCodeLookup()
            {
                InitErrorCodes();
            }

            public class ErrorCode
            {
                public string Code;
                public string Message;

                public ErrorCode(string code, string message)
                {
                    Code = code;
                    Message = message;
                }
            }

            public static ErrorCode Lookup(int code)
            {
                InitErrorCodes();

                try
                {
                    var keyValuePair = new KeyValuePair<int, string>(code, _errorCodes[code]);

                    return new ErrorCode ($"E-{keyValuePair.Key}", $"{keyValuePair.Value}");
                }

                catch (Exception ex)
                {
                    Helper.WriteDebug(ex);
                }

                return null;
            }

            protected static void InitErrorCodes()
            {
                int _base = 0x00;

                _errorCodes = new Dictionary<int, string>
                {
                    // section 0
                    {_base + 0x00,  "*** Device OK ***" },
                    {_base + 0x01,  "Device beginning flush" },
                    {_base + 0x02,  "Ready for test" },
                    {_base + 0x03,  "Beginning breath maneuver" },
                    {_base + 0x04,  "Stop breath maneuver OK" },
                    {_base + 0x05,  "Calculating test results (\"Device is starting T4\")" },
                    {_base + 0x06,  "Calculating test results failed (\"Device is starting T4\")" },
                    {_base + 0x07,  "Tested results calculated succesfully" },
                    {_base + 0x08,  "POST checksum wrong" },
                    {_base + 0x09,  "POST error from any other fault" },
                    {_base + 0x0A,  "Device entering sleep mode" },
                    {_base + 0x0B,  "Device is not in a valid operating state" },
                    {_base + 0x0C,  "Device expiry is <= 60days" },
                    {_base + 0x0D,  "F150 is beyond its usable life.  Contact Customer Support for replacement." },
                    {_base + 0x0E,  "Sytem Error.  If restarting does not correct error code, Report error code to Customer Service." },
                    {_base + 0x0F,  "undefined: 0x0f" },

                    // section 1
                    {_base + 0x10,  "*** Section 1 ***" },
                    {_base + 0x11,  "Sample flow is out of tolerance" },
                    {_base + 0x12,  "undecl: 0x12" },
                    {_base + 0x13,  "undecl: 0x13" },
                    {_base + 0x14,  "undecl: 0x14" },
                    {_base + 0x15,  "undecl: 0x15" },
                    {_base + 0x16,  "undecl: 0x16" },
                    {_base + 0x17,  "undecl: 0x17" },
                    {_base + 0x18,  "undecl: 0x18" },
                    {_base + 0x19,  "undecl: 0x19" },
                    {_base + 0x1A,  "undecl: 0x1A" },
                    {_base + 0x1B,  "undecl: 0x1B" },
                    {_base + 0x1C,  "undecl: 0x1C" },
                    {_base + 0x1D,  "undecl: 0x1D" },
                    {_base + 0x1E,  "undecl: 0x1E" },
                    {_base + 0x1F,  "undecl: 0x1F" },

                    // section 2
                    {_base + 0x20,  "*** All sensors in operating range (Temp, Hum, Pres) ***" },
                    {_base + 0x21,  "Temperature > 35.4° C" },
                    {_base + 0x22,  "Temperature < 14.6° C" },
                    {_base + 0x23,  "Humidity >92% RH" },
                    {_base + 0x24,  "Humidity <18% RH" },
                    {_base + 0x25,  "Ambient pressure < 75.35 kPa" },
                    {_base + 0x26,  "Ambient pressure > 110.65 kPa" },
                    {_base + 0x27,  "Humidity indicator turns Yellow.\r\nstatus screen indicates \"F150 humidity is near low limit. Move to a higher humidity location and allow time to adjust.\"" },
                    {_base + 0x28,  "undefined: 0x18" },
                    {_base + 0x29,  "Do we want to give edge case warning for all environmental sensors?" },
                    {_base + 0x2A,  "Do we want to give edge case warning for NO sensor?  Also envrinmntal errors should cause NO sensor indicator errors?" },
                    {_base + 0x2B,  "undecl: 0x2B" },
                    {_base + 0x2C,  "undecl: 0x2C" },
                    {_base + 0x2D,  "undecl: 0x2D" },
                    {_base + 0x2E,  "undecl: 0x2E" },
                    {_base + 0x2F,  "undecl: 0x2F" },

                    // section 3
                    {_base + 0x30,  "*** Breath Flow ***" },
                    {_base + 0x31,  "Breath flow was above maximum.   Try again." },
                    {_base + 0x32,  "Breath flow was too high. Focus on the star." },
                    {_base + 0x33,  "Breath flow was too low. Focus on the star." },
                    {_base + 0x34,  "Breath flow was unstable. Provide a steady and smooth flow." },
                    {_base + 0x35,  "Breath flow did not stop fast enough.  Let go immediately at STOP." },
                    {_base + 0x36,  "Breath flow detected after STOP." },
                    {_base + 0x37,  "undecl: 0x37" },
                    {_base + 0x38,  "undecl: 0x38" },
                    {_base + 0x39,  "undecl: 0x39" },
                    {_base + 0x3A,  "undecl: 0x3A" },
                    {_base + 0x3B,  "undecl: 0x3B" },
                    {_base + 0x3C,  "undecl: 0x3C" },
                    {_base + 0x3D,  "undecl: 0x3D" },
                    {_base + 0x3E,  "undecl: 0x3E" },
                    {_base + 0x3F,  "undecl: 0x3F" },

                    // section 4
                    {_base + 0x40,  "*** Battery voltage too high ***" },
                    {_base + 0x41,  "Battery voltage is low.  Report error code to Customer Service." },
                    {_base + 0x42,  "Battery charge is low. Connect F150 to USB-C (charge cord)." },
                    {_base + 0x43,  "Battery >20% charge" },
                    {_base + 0x44,  "battery <20% charge" },
                    {_base + 0x45,  "battery <5% charge" },
                    {_base + 0x46,  "undecl: 0x46" },
                    {_base + 0x47,  "undecl: 0x47" },
                    {_base + 0x48,  "undecl: 0x48" },
                    {_base + 0x49,  "undecl: 0x49" },
                    {_base + 0x4A,  "undecl: 0x4A" },
                    {_base + 0x4B,  "undecl: 0x4B" },
                    {_base + 0x4C,  "undecl: 0x4C" },
                    {_base + 0x4D,  "undecl: 0x4D" },
                    {_base + 0x4E,  "undecl: 0x4E" },
                    {_base + 0x4F,  "undecl: 0x4F" },

                    // section 5
                    {_base + 0x50,  "*** no device in range (BTLE) or connected ***" },
                    {_base + 0x51,  "BTLE is not functioning correctly. Report error code to Customer Service." },
                    {_base + 0x52,  "App handshake not completed" },
                    {_base + 0x53,  "undecl: 0x53" },
                    {_base + 0x54,  "undecl: 0x54" },
                    {_base + 0x55,  "undecl: 0x55" },
                    {_base + 0x56,  "undecl: 0x56" },
                    {_base + 0x57,  "undecl: 0x57" },
                    {_base + 0x58,  "undecl: 0x58" },
                    {_base + 0x59,  "undecl: 0x59" },
                    {_base + 0x5A,  "undecl: 0x5A" },
                    {_base + 0x5B,  "undecl: 0x5B" },
                    {_base + 0x5C,  "undecl: 0x5C" },
                    {_base + 0x5D,  "undecl: 0x5D" },
                    {_base + 0x5E,  "undecl: 0x5E" },
                    {_base + 0x5F,  "undecl: 0x5F" },

                    // section 6
                    {_base + 0x60,  "*** Cannot communicate with SPI Flash ***" },
                    {_base + 0x61,  "SPI Flash error returned" },
                    {_base + 0x62,  "Flash CRC checksum failed - memory corrupt" },
                    {_base + 0x63,  "Memory full" },
                    {_base + 0x64,  "undecl: 0x64" },
                    {_base + 0x65,  "undecl: 0x65" },
                    {_base + 0x66,  "undecl: 0x66" },
                    {_base + 0x67,  "undecl: 0x67" },
                    {_base + 0x68,  "undecl: 0x68" },
                    {_base + 0x69,  "undecl: 0x69" },
                    {_base + 0x6A,  "undecl: 0x6A" },
                    {_base + 0x6B,  "undecl: 0x6B" },
                    {_base + 0x6C,  "undecl: 0x6C" },
                    {_base + 0x6D,  "undecl: 0x6D" },
                    {_base + 0x6E,  "undecl: 0x6E" },
                    {_base + 0x6F,  "undecl: 0x6F" },

                    // section 7
                    {_base + 0x70,  "*** NO Sensor is missing.  Install a F150 sensor. ***" },
                    {_base + 0x71,  "NO Sensor communication failed." },
                    {_base + 0x72,  "NO Sensor is within 60 days of expiration" },
                    {_base + 0x73,  "NO Sensor is expired.  Install a new NO sensor." },
                    {_base + 0x74,  "NO sensor is not functioning correctly.  Report error code to Customer Service." },
                    {_base + 0x75,  "Breath flow sensor is not functioning correctly.  Report error code to Customer Service." },
                    {_base + 0x76,  "Sample flow sensor is not functioning correctly.  Report error code to Customer Service." },
                    {_base + 0x77,  "Ambient Pressure sensor is not functioning correctly.  Report error code to Customer Service." },
                    {_base + 0x78,  "Humidity sensor is not functioning correctly.  Report error code to Customer Service." },
                    {_base + 0x79,  "undecl: 0x79" },
                    {_base + 0x7A,  "undecl: 0x7A" },
                    {_base + 0x7B,  "undecl: 0x7B" },
                    {_base + 0x7C,  "undecl: 0x7C" },
                    {_base + 0x7D,  "undecl: 0x7D" },
                    {_base + 0x7E,  "undecl: 0x7E" },
                    {_base + 0x7F,  "undecl: 0x7F" },
                };
            }
        }
    }
}