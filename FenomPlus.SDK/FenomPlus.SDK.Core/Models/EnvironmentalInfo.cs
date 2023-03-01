using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace FenomPlus.SDK.Core.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class EnvironmentalInfo : BaseCharacteristic
    {
        private const int COMM_AMBIENT_TEMPERATURE_ID     = (0x20);
        private const int COMM_AMBIENT_TEMPERATURE_SIZE   = 4;
        private const int COMM_AMBIENT_PRESSURE_ID        = (0x21);
        private const int COMM_AMBIENT_PRESSURE_SIZE      = 4;
        private const int COMM_AMBIENT_HUMIDITY_ID        = (0x22);
        private const int COMM_AMBIENT_HUMIDITY_SIZE      = 4;
        private const int COMM_BATTERY_LEVEL_ID           = (0x23);
        private const int COMM_BATTERY_LEVEL_SIZE         = 4;
        private const int COMM_ENVIRONMENTAL_ITEMS        = 4;
        private const int COMM_ENVIRONMENTAL_PAYLOAD_SIZE = (COMM_AMBIENT_TEMPERATURE_SIZE
                                                            + COMM_AMBIENT_PRESSURE_SIZE
                                                            + COMM_AMBIENT_HUMIDITY_SIZE
                                                            + COMM_BATTERY_LEVEL_SIZE);

        public float Temperature;
        public float Pressure;
        public float Humidity;
        public int BatteryLevel;

        public EnvironmentalInfo Decode(byte[] data)
        {
            int totalSize = COMM_ENVIRONMENTAL_PAYLOAD_SIZE + (COMM_ENVIRONMENTAL_ITEMS * 2) + 1;

            if (data.Length != totalSize)
                throw new ArgumentException($"Payload size mismatch (expected: {totalSize}, saw: {data.Length})");

            int offset = 0;

            int itemCount = data[offset++];

            if (itemCount != COMM_ENVIRONMENTAL_ITEMS)
                throw new ArgumentException($"Payload count mismatch (expected: {COMM_ENVIRONMENTAL_ITEMS}, saw: {itemCount})");

            while (offset < totalSize)
            {
                int type = data[offset++];
                int size = data[offset++];

                switch (type)
                {
                    case COMM_AMBIENT_TEMPERATURE_ID:
                        if (size != COMM_AMBIENT_TEMPERATURE_SIZE)
                        {
                            throw new ArgumentException($"Unexpected payload item size (expected: {COMM_AMBIENT_TEMPERATURE_SIZE}, saw: {size})");
                        }
                        Temperature = ToFloat(data,offset);
                        break;

                    case COMM_AMBIENT_PRESSURE_ID:
                        if (size != COMM_AMBIENT_PRESSURE_SIZE)
                        {
                            throw new ArgumentException($"Unexpected payload item size (expected: {COMM_AMBIENT_PRESSURE_SIZE}, saw: {size})");
                        }
                        Pressure = ToFloat(data, offset) / 1000;
                        break;

                    case COMM_AMBIENT_HUMIDITY_ID:
                        if (size != COMM_AMBIENT_HUMIDITY_SIZE)
                        {
                            throw new ArgumentException($"Unexpected payload item size (expected: {COMM_AMBIENT_HUMIDITY_SIZE}, saw: {size})");
                        }
                        Humidity = ToFloat(data, offset);
                        break;

                    case COMM_BATTERY_LEVEL_ID:
                        if (size != COMM_BATTERY_LEVEL_SIZE)
                        {
                            throw new ArgumentException($"Unexpected payload item size (expected: {COMM_BATTERY_LEVEL_SIZE}, saw: {size})");
                        }

                        BatteryLevel = (short) ToFloat(data, offset);
                        break;
                }

                offset += size;
            }

            return this;
        }

        public EnvironmentalInfo()
        {
        }

        public static EnvironmentalInfo Create(byte[] data)
        {
            EnvironmentalInfo environmentalInfo = new EnvironmentalInfo();
            return environmentalInfo.Decode(data);
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
