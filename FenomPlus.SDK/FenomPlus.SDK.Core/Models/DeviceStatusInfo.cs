using System;
using System.Runtime.InteropServices;

namespace FenomPlus.SDK.Core.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class DeviceStatusInfo : BaseCharacteristic
    {
        public byte StatusCode;

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

                if (data != null)
                {
                    StatusCode = Data[0];
                }
            }

            finally
            {
            }


            return this;
        }

    }
}
