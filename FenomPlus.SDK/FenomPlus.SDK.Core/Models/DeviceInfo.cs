using System;
using System.Runtime.InteropServices;
using FenomPlus.Services;

namespace FenomPlus.SDK.Core.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class DeviceInfo : BaseCharacteristic
    {
        public static int Min = 17;

        /// <summary>
        /// 
        /// </summary>
        public byte DeviceStatus;
        public byte MajorVersion;
        public byte MinorVersion;
        public byte BuildVersion;
        public byte SensorExpDateMonth; 
        public byte SensorExpDateDay;
        public short SensorExpDateYear;
        public byte[] SerialNumber;             // 10 bytes

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DeviceInfo Create(byte[] data)
        {
            DeviceInfo deviceInfo = new DeviceInfo();
            return deviceInfo.Decode(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public DeviceInfo Decode(byte[] data)
        {
            try
            {
                Data = data;
                if ((data != null) && (data.Length >= Min))
                {
                    DeviceStatus = Data[0];
                    MajorVersion = Data[1];
                    MinorVersion = Data[2];
                    BuildVersion = Data[3];
                    SensorExpDateMonth = Data[4];
                    SensorExpDateDay = Data[5];
                    SensorExpDateYear = (short)((int)(((int)Data[6]) * 100 + (int)Data[7]));
                    int len = data.Length - 8;
                    SerialNumber = new byte[len];
                    Array.Copy(data, 8, SerialNumber, 0, len);
                }
            } finally { }
            return this;
        }
    }
}
