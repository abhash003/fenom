using System;
using System.Runtime.InteropServices;

namespace FenomPlus.SDK.Core.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class ErrorStatusInfo : BaseCharacteristic
    {
        public static int Min = 5;

        public byte ErrorCode;
        public float FutureValue;

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

                if ((data != null) && (data.Length >= Min))
                {
                    ErrorCode = Data[0];
                    FutureValue = System.BitConverter.ToSingle(data, 1);
                }
            }
            finally { }

            return this;
        }

    }
}
