using System;

namespace FenomPlus.SDK.Core.Features
{
    public enum ID_SUB
    {
        // ID_REQUEST_DATA
        ID_REQUEST_FENONSERVICE = 0,
        ID_REQUEST_DEVICEINFO = 1,
        ID_REQUEST_ENVIROMENTALINFO = 2,
        ID_REQUEST_BREATHTEST = 3,
        ID_REQUEST_BREATHMANUEVER = 4,
        ID_REQUEST_TRAININGMODE = 5,
        ID_REQUEST_UNKNOWN = 6,
        ID_REQUEST_DEBUGMSG = 7,
        ID_REQUEST_DEBUGMANUEVERTYPE = 8,
        ID_REQUEST_DATETIME = 9,
        ID_REQUEST_CALIBRATION = 10,

        // ID_PROVISIONING_DATA
        ID_PROVISIONING_SERIALNUMBER = 0,   // 8 byte ascii serail unmber
        ID_PROVISIONING_DATETIME = 1,       // HEX MMDDYYYYHHMMSS = 7 bytes hex nibble

        // ID_CALIBRATION_DATA
        ID_CALIBRATION = 0,                 // 2 bytes (signed int) * 3 = 6 bytes with 2 bytes spar
    }
}
