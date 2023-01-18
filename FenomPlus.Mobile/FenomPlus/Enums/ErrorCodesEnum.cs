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
            private static Dictionary<int, string> _errorCodes;

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
                    {_base + 0x07,  "Tested results calculated successfully" },
                    {_base + 0x08,  "POST checksum wrong" },
                    {_base + 0x09,  "POST error from any other fault" },
                    {_base + 0x0A,  "Device entering sleep mode" },
                    {_base + 0x0B,  "Device is not in a valid operating state" },
                    {_base + 0x0C,  "Device expiry is <= 60days" },
                    {_base + 0x0D,  "F150 is beyond its usable life.  Contact Customer Support for replacement." },
                    {_base + 0x0E,  "System Error.  If restarting does not correct error code, Report error code to Customer Service." },
                    {_base + 0x0F,  "undefined: 0x0f" },

                    // section 1
                    {_base + 0x100,  "*** Section 1 ***" },
                    {_base + 0x101,  "Sample flow is out of tolerance" },
                    {_base + 0x102,  "undecl: 0x102" },
                    {_base + 0x103,  "undecl: 0x103" },
                    {_base + 0x104,  "undecl: 0x104" },
                    {_base + 0x105,  "undecl: 0x105" },
                    {_base + 0x106,  "undecl: 0x106" },
                    {_base + 0x107,  "undecl: 0x107" },
                    {_base + 0x108,  "undecl: 0x108" },
                    {_base + 0x109,  "undecl: 0x109" },
                    {_base + 0x10A,  "undecl: 0x10A" },
                    {_base + 0x10B,  "undecl: 0x10B" },
                    {_base + 0x10C,  "undecl: 0x10C" },
                    {_base + 0x10D,  "undecl: 0x10D" },
                    {_base + 0x10E,  "undecl: 0x10E" },
                    {_base + 0x10F,  "undecl: 0x10F" },

                    // section 2
                    {_base + 0x200,  "*** All sensors in operating range (Temp, Hum, Pres) ***" },
                    {_base + 0x201,  "Temperature > 35.4° C" },
                    {_base + 0x202,  "Temperature < 14.6° C" },
                    {_base + 0x203,  "Humidity >92% RH" },
                    {_base + 0x204,  "Humidity <18% RH" },
                    {_base + 0x205,  "Ambient pressure < 75.35 kPa" },
                    {_base + 0x206,  "Ambient pressure > 110.65 kPa" },
                    {_base + 0x207,  "Humidity indicator turns Yellow.\r\nstatus screen indicates \"F150 humidity is near low limit. Move to a higher humidity location and allow time to adjust.\"" },
                    {_base + 0x208,  "undecl: 0x28" },
                    {_base + 0x209,  "Do we want to give edge case warning for all environmental sensors?" },
                    {_base + 0x20A,  "Do we want to give edge case warning for NO sensor?  Also envrinmntal errors should cause NO sensor indicator errors?" },
                    {_base + 0x20B,  "undecl: 0x2B" },
                    {_base + 0x20C,  "undecl: 0x2C" },
                    {_base + 0x20D,  "undecl: 0x2D" },
                    {_base + 0x20E,  "undecl: 0x2E" },
                    {_base + 0x20F,  "undecl: 0x2F" },

                    // section 3
                    {_base + 0x300,  "*** Breath Flow ***" },
                    {_base + 0x301,  "Breath flow was above maximum. Try again." },
                    {_base + 0x302,  "Breath flow was too high. Focus on the star." },
                    {_base + 0x303,  "Breath flow was too low. Focus on the star." },
                    {_base + 0x304,  "Breath flow was unstable. Provide a steady and smooth flow." },
                    {_base + 0x305,  "Breath flow did not stop fast enough. Let go immediately at STOP." },
                    {_base + 0x306,  "Breath flow detected after STOP." },
                    {_base + 0x307,  "undecl: 0x37" },
                    {_base + 0x308,  "undecl: 0x38" },
                    {_base + 0x309,  "undecl: 0x39" },
                    {_base + 0x30A,  "undecl: 0x3A" },
                    {_base + 0x30B,  "undecl: 0x3B" },
                    {_base + 0x30C,  "undecl: 0x3C" },
                    {_base + 0x30D,  "undecl: 0x3D" },
                    {_base + 0x30E,  "undecl: 0x3E" },
                    {_base + 0x30F,  "undecl: 0x3F" },

                    // section 4
                    {_base + 0x400,  "*** Battery voltage too high ***" },
                    {_base + 0x401,  "Battery voltage is low.  Report error code to Customer Service." },
                    {_base + 0x402,  "Battery charge is low. Connect F150 to USB-C (charge cord)." },
                    {_base + 0x403,  "Battery >20% charge" },
                    {_base + 0x404,  "battery <20% charge" },
                    {_base + 0x405,  "battery <5% charge" },
                    {_base + 0x406,  "undecl: 0x46" },
                    {_base + 0x407,  "undecl: 0x47" },
                    {_base + 0x408,  "undecl: 0x48" },
                    {_base + 0x409,  "undecl: 0x49" },
                    {_base + 0x40A,  "undecl: 0x4A" },
                    {_base + 0x40B,  "undecl: 0x4B" },
                    {_base + 0x40C,  "undecl: 0x4C" },
                    {_base + 0x40D,  "undecl: 0x4D" },
                    {_base + 0x40E,  "undecl: 0x4E" },
                    {_base + 0x40F,  "undecl: 0x4F" },

                    // section 5
                    {_base + 0x500,  "*** no device in range (BTLE) or connected ***" },
                    {_base + 0x501,  "BTLE is not functioning correctly. Report error code to Customer Service." },
                    {_base + 0x502,  "App handshake not completed" },
                    {_base + 0x503,  "undecl: 0x53" },
                    {_base + 0x504,  "undecl: 0x54" },
                    {_base + 0x505,  "undecl: 0x55" },
                    {_base + 0x506,  "undecl: 0x56" },
                    {_base + 0x507,  "undecl: 0x57" },
                    {_base + 0x508,  "undecl: 0x58" },
                    {_base + 0x509,  "undecl: 0x59" },
                    {_base + 0x50A,  "undecl: 0x5A" },
                    {_base + 0x50B,  "undecl: 0x5B" },
                    {_base + 0x50C,  "undecl: 0x5C" },
                    {_base + 0x50D,  "undecl: 0x5D" },
                    {_base + 0x50E,  "undecl: 0x5E" },
                    {_base + 0x50F,  "undecl: 0x5F" },

                    // section 6
                    {_base + 0x600,  "*** Cannot communicate with SPI Flash ***" },
                    {_base + 0x601,  "SPI Flash error returned" },
                    {_base + 0x602,  "Flash CRC checksum failed - memory corrupt" },
                    {_base + 0x603,  "Memory full" },
                    {_base + 0x604,  "undecl: 0x64" },
                    {_base + 0x605,  "undecl: 0x65" },
                    {_base + 0x606,  "undecl: 0x66" },
                    {_base + 0x607,  "undecl: 0x67" },
                    {_base + 0x608,  "undecl: 0x68" },
                    {_base + 0x609,  "undecl: 0x69" },
                    {_base + 0x60A,  "undecl: 0x6A" },
                    {_base + 0x60B,  "undecl: 0x6B" },
                    {_base + 0x60C,  "undecl: 0x6C" },
                    {_base + 0x60D,  "undecl: 0x6D" },
                    {_base + 0x60E,  "undecl: 0x6E" },
                    {_base + 0x60F,  "undecl: 0x6F" },

                    // section 7
                    {_base + 0x700,  "*** NO Sensor is missing.  Install a F150 sensor. ***" },
                    {_base + 0x701,  "NO Sensor communication failed." },
                    {_base + 0x702,  "NO Sensor is within 60 days of expiration" },
                    {_base + 0x703,  "NO Sensor is expired.  Install a new NO sensor." },
                    {_base + 0x704,  "NO sensor is not functioning correctly.  Report error code to Customer Service." },
                    {_base + 0x705,  "Breath flow sensor is not functioning correctly.  Report error code to Customer Service." },
                    {_base + 0x706,  "Sample flow sensor is not functioning correctly.  Report error code to Customer Service." },
                    {_base + 0x707,  "Ambient Pressure sensor is not functioning correctly.  Report error code to Customer Service." },
                    {_base + 0x708,  "Humidity sensor is not functioning correctly.  Report error code to Customer Service." },
                    {_base + 0x709,  "undecl: 0x79" },
                    {_base + 0x70A,  "undecl: 0x7A" },
                    {_base + 0x70B,  "undecl: 0x7B" },
                    {_base + 0x70C,  "undecl: 0x7C" },
                    {_base + 0x70D,  "undecl: 0x7D" },
                    {_base + 0x70E,  "undecl: 0x7E" },
                    {_base + 0x70F,  "undecl: 0x7F" },
                };
            }
        }
    }
}