﻿using System;
using System.Runtime.InteropServices;

namespace FenomPlus.SDK.Core.Models.Characteristic
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class EnvironmentalInfo : BaseCharacteristic
    {
        public static int Min = 3;

        public byte Temperature;
        public byte Humidity;
        public byte Pressure;
        public byte BatteryLevel;       // new version

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static EnvironmentalInfo Create(byte[] data)
        {
            EnvironmentalInfo environmentalInfo = new EnvironmentalInfo();
            return environmentalInfo.Decode(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public EnvironmentalInfo Decode(byte[] data)
        {
            Data = data;
            if ((data != null) && (data.Length >= Min))
            {
                Temperature = Data[0];
                Humidity = Data[1];
                Pressure = Data[2];
                if (NewVersion && (data.Length != Min))
                {
                    BatteryLevel = Data[3];
                    App.BatteryLevel = BatteryLevel;
                }
            }
            return this;
        }
    }
}
