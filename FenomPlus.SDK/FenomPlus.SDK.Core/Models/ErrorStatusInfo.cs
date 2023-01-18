using System;
using System.Runtime.InteropServices;
using FenomPlus.Services.DeviceService.Utils;

namespace FenomPlus.SDK.Core.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class ErrorStatusInfo : BaseCharacteristic
    {
        public byte ErrorCode;

        public static ErrorStatusInfo Create(byte[] data)
        {
            ErrorStatusInfo errorStatusInfo = new ErrorStatusInfo();
            return errorStatusInfo.Decode(data);
        }

        public ErrorStatusInfo Decode(byte[] data)
        {
            try
            {
                Data = data;

                if (data != null)
                {
                    ErrorCode = Data[0];
                }
            }

            finally
            {
            }

            // add logging here
            //Helper.WriteDebug("");

            return this;
        }

    }
}
