using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace FenomPlus.SDK.Core.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class EnvironmentalInfo : BaseCharacteristic
    {
        // ToDo: New Code
        //public static int Min = 13;
        public static int Min = 4;

        // ToDo: New Code
        //public float Temperature;
        //public float Humidity;
        //public float Pressure;
        //public byte BatteryLevel;

        public byte Temperature;
        public byte Humidity;
        public byte Pressure;
        public byte BatteryLevel;

        public EnvironmentalInfo()
        {
            Humidity = 100;
        }

        public static EnvironmentalInfo Create(byte[] data)
        {
            EnvironmentalInfo environmentalInfo = new EnvironmentalInfo();
            return environmentalInfo.Decode(data);
        }

        public EnvironmentalInfo Decode(byte[] data)
        {
            try
            {
                Data = data;

                // ToDo: New Code
                //if ((data != null) && (data.Length >= Min))
                //{
                //    Temperature = System.BitConverter.ToSingle(data, 0);
                //    Humidity = System.BitConverter.ToSingle(data, 4);
                //    Pressure = System.BitConverter.ToSingle(data, 8);
                //    BatteryLevel = Data[12];
                //}

                if ((data != null) && (data.Length >= Min))
                {
                    Temperature = Data[0];
                    Humidity = Data[1];
                    Pressure = Data[2];
                    BatteryLevel = Data[3];
                }
            }
            finally { }

            return this;
        }

        //Little-endian D2-02-96-49
        //Big-endian	49-96-02-D2

        // Code to swap endians...
        public float floatConversion(byte[] bytes)
        {
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes); // Convert big endian to little endian
            }

            float myFloat = BitConverter.ToSingle(bytes, 0);
            return myFloat;
        }

    }
}


// Code to swap endians...

//public float floatConversion(byte[] bytes)
//{
//    if (BitConverter.IsLittleEndian)
//    {
//        Array.Reverse(bytes); // Convert big endian to little endian
//    }
//    float myFloat = BitConverter.ToSingle(bytes, 0);
//    return myFloat;
//}


// Original array
// Byte[] data = new Byte[] { 0x42, 0x29, 0xEC, 0x00 };
// 42.48047
// If CPU uses Little Endian, we should reverse the data 
// float result = BitConverter.ToSingle(BitConverter.IsLittleEndian ? data.Reverse().ToArray() : data, 0);
