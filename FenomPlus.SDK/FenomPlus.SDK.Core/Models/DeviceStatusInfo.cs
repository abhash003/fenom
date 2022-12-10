using System;
using System.Runtime.InteropServices;

namespace FenomPlus.SDK.Core.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class DeviceStatusInfo : BaseCharacteristic
    {
        public static int Min = 5;

        public byte StatusCode;
        public float FutureValue;

        public static DeviceStatusInfo Create(byte[] data)
        {
            DeviceStatusInfo deviceStatusInfo = new DeviceStatusInfo();
            return deviceStatusInfo.Decode(data);
        }

        public DeviceStatusInfo Decode(byte[] data)
        {
            try
            {
                Data = data;

                if ((data != null) && (data.Length >= Min))
                {
                    StatusCode = Data[0];
                    FutureValue = System.BitConverter.ToSingle(data, 1);
                }
            }
            finally { }

            return this;
        }

    }
}
