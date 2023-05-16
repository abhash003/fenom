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
            private readonly static Dictionary<int, string> _errorCodes = new Dictionary<int, string>
            {
                    // section 0
                    { 0x00,  "*** Device OK ***" },
                    { 0x01,  "Device beginning flush" },
                    { 0x02,  "Ready for test" },
                    { 0x03,  "Beginning breath maneuver" },
                    { 0x04,  "Stop breath maneuver OK" },
                    { 0x05,  "Calculating test results (\"Device is starting T4\")" },
                    { 0x06,  "Calculating test results failed (\"Device is starting T4\")" },
                    { 0x07,  "Tested results calculated successfully" },
                    { 0x08,  "POST checksum wrong" },
                    { 0x09,  "POST error from any other fault" },
                    { 0x0A,  "Device entering sleep mode" },
                    { 0x0B,  "Device is not in a valid operating state" },
                    { 0x0C,  "Device expiry is <= 60days" },
                    { 0x0D,  "F150 is beyond its usable life.  Contact Customer Support for replacement." },
                    { 0x0E,  "System Error.  If restarting does not correct error code, Report error code to Customer Service." },
                    { 0x0F,  "undefined: 0x0f" },

                    // section 1
                    { 0x10,  "*** Section 1 ***" },
                    { 0x11,  "Sample flow is out of tolerance" },
                    { 0x12,  "undecl: 0x102" },
                    { 0x13,  "undecl: 0x103" },
                    { 0x14,  "undecl: 0x104" },
                    { 0x15,  "undecl: 0x105" },
                    { 0x16,  "undecl: 0x106" },
                    { 0x17,  "undecl: 0x107" },
                    { 0x18,  "undecl: 0x108" },
                    { 0x19,  "undecl: 0x109" },
                    { 0x1A,  "undecl: 0x10A" },
                    { 0x1B,  "undecl: 0x10B" },
                    { 0x1C,  "undecl: 0x10C" },
                    { 0x1D,  "undecl: 0x10D" },
                    { 0x1E,  "undecl: 0x10E" },
                    { 0x1F,  "undecl: 0x10F" },

                    // section 2
                    { 0x20,  "*** All sensors in operating range (Temp, Hum, Pres) ***" },
                    { 0x21,  "Temperature > 35.4° C" },
                    { 0x22,  "Temperature < 14.6° C" },
                    { 0x23,  "Humidity >92% RH" },
                    { 0x24,  "Humidity <18% RH" },
                    { 0x25,  "Ambient pressure < 75.35 kPa" },
                    { 0x26,  "Ambient pressure > 110.65 kPa" },
                    { 0x27,  "Humidity indicator turns Yellow.\r\nstatus screen indicates \"F150 humidity is near low limit. Move to a higher humidity location and allow time to adjust.\"" },
                    { 0x28,  "undecl: 0x28" },
                    { 0x29,  "Do we want to give edge case warning for all environmental sensors?" },
                    { 0x2A,  "Do we want to give edge case warning for NO sensor?  Also envrinmntal errors should cause NO sensor indicator errors?" },
                    { 0x2B,  "undecl: 0x2B" },
                    { 0x2C,  "undecl: 0x2C" },
                    { 0x2D,  "undecl: 0x2D" },
                    { 0x2E,  "undecl: 0x2E" },
                    { 0x2F,  "undecl: 0x2F" },

                    // section 3
                    { 0x30,  "*** Breath Flow ***" },
                    { 0x31,  "Breath flow was above maximum. Try again." },
                    { 0x32,  "Breath flow was below minimum. Try again." },
                    { 0x33,  "Breath flow was too high. Focus on the star." },
                    { 0x34,  "Breath flow was too low. Focus on the star." },
                    { 0x35,  "Breath flow was unstable. Provide a steady and smooth flow." },
                    { 0x36,  "Breath flow did not stop fast enough. Let go immediately at STOP." },
                    { 0x37,  "Breath flow detected after STOP." },
                    { 0x38,  "undecl: 0x38" },
                    { 0x39,  "undecl: 0x39" },
                    { 0x3A,  "undecl: 0x3A" },
                    { 0x3B,  "undecl: 0x3B" },
                    { 0x3C,  "undecl: 0x3C" },
                    { 0x3D,  "undecl: 0x3D" },
                    { 0x3E,  "undecl: 0x3E" },
                    { 0x3F,  "undecl: 0x3F" },

                    // section 4
                    { 0x40,  "*** Battery voltage too high ***" },
                    { 0x41,  "Battery voltage is low.  Report error code to Customer Service." },
                    { 0x42,  "Battery charge is low. Connect F150 to USB-C (charge cord)." },
                    { 0x43,  "Battery >20% charge" },
                    { 0x44,  "battery <20% charge" },
                    { 0x45,  "battery <5% charge" },
                    { 0x46,  "undecl: 0x46" },
                    { 0x47,  "undecl: 0x47" },
                    { 0x48,  "undecl: 0x48" },
                    { 0x49,  "undecl: 0x49" },
                    { 0x4A,  "undecl: 0x4A" },
                    { 0x4B,  "undecl: 0x4B" },
                    { 0x4C,  "undecl: 0x4C" },
                    { 0x4D,  "undecl: 0x4D" },
                    { 0x4E,  "undecl: 0x4E" },
                    { 0x4F,  "undecl: 0x4F" },

                    // section 5
                    { 0x50,  "*** no device in range (BTLE) or connected ***" },
                    { 0x51,  "BTLE is not functioning correctly. Report error code to Customer Service." },
                    { 0x52,  "App handshake not completed" },
                    { 0x53,  "undecl: 0x53" },
                    { 0x54,  "undecl: 0x54" },
                    { 0x55,  "undecl: 0x55" },
                    { 0x56,  "undecl: 0x56" },
                    { 0x57,  "undecl: 0x57" },
                    { 0x58,  "undecl: 0x58" },
                    { 0x59,  "undecl: 0x59" },
                    { 0x5A,  "undecl: 0x5A" },
                    { 0x5B,  "undecl: 0x5B" },
                    { 0x5C,  "undecl: 0x5C" },
                    { 0x5D,  "undecl: 0x5D" },
                    { 0x5E,  "undecl: 0x5E" },
                    { 0x5F,  "undecl: 0x5F" },

                    // section 6
                    { 0x60,  "*** Cannot communicate with SPI Flash ***" },
                    { 0x61,  "SPI Flash error returned" },
                    { 0x62,  "Flash CRC checksum failed - memory corrupt" },
                    { 0x63,  "Memory full" },
                    { 0x64,  "undecl: 0x64" },
                    { 0x65,  "undecl: 0x65" },
                    { 0x66,  "undecl: 0x66" },
                    { 0x67,  "undecl: 0x67" },
                    { 0x68,  "undecl: 0x68" },
                    { 0x69,  "undecl: 0x69" },
                    { 0x6A,  "undecl: 0x6A" },
                    { 0x6B,  "undecl: 0x6B" },
                    { 0x6C,  "undecl: 0x6C" },
                    { 0x6D,  "undecl: 0x6D" },
                    { 0x6E,  "undecl: 0x6E" },
                    { 0x6F,  "undecl: 0x6F" },

                    // section 7
                    { 0x70,  "*** NO Sensor is missing.  Install a F150 sensor. ***" },
                    { 0x71,  "NO Sensor communication failed." },
                    { 0x72,  "NO Sensor is within 60 days of expiration" },
                    { 0x73,  "NO Sensor is expired.  Install a new NO sensor." },
                    { 0x74,  "NO sensor is not functioning correctly.  Report error code to Customer Service." },
                    { 0x75,  "Breath flow sensor is not functioning correctly.  Report error code to Customer Service." },
                    { 0x76,  "Sample flow sensor is not functioning correctly.  Report error code to Customer Service." },
                    { 0x77,  "Ambient Pressure sensor is not functioning correctly.  Report error code to Customer Service." },
                    { 0x78,  "Humidity sensor is not functioning correctly.  Report error code to Customer Service." },
                    { 0x79,  "undecl: 0x79" },
                    { 0x7A,  "undecl: 0x7A" },
                    { 0x7B,  "undecl: 0x7B" },
                    { 0x7C,  "undecl: 0x7C" },
                    { 0x7D,  "undecl: 0x7D" },
                    { 0x7E,  "undecl: 0x7E" },
                    { 0x7F,  "undecl: 0x7F" },
                    // section 8
                    { 0x81,  "QC Negative Control Test failed, disable QC, then enable QC will rescue" },
            };

            public ErrorCodeLookup()
            {
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

            public static ErrorCode Lookup(int firmwareCode)
            {
                try
                {
                    var keyValuePair = new KeyValuePair<int, string>(firmwareCode, _errorCodes[firmwareCode]);

                    var displayCode = ((firmwareCode & 0xf0) << 4) | (firmwareCode & 0x0f);
                    
                    return new ErrorCode ($"E-{displayCode.ToString("X")}", $"{keyValuePair.Value}");
                }

                catch (Exception ex)
                {
                    Helper.WriteDebug(ex);
                }

                return null;
            }
        }
    }
}