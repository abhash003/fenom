using System;

namespace FenomPlus.SDK.Core.Models
{
    public class BaseCharacteristic
    {
        // store orginal data
        protected byte[] Data;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        protected float ExtractFloat(byte[] data, int index)
        {
            uint val = 0;

            val += (uint)data[index + 0] * 256 * 256 * 256;
            val += (uint)data[index + 1] * 256 * 256;
            val += (uint)data[index + 2] * 256;
            val += (uint)data[index + 3];

            return (float)val;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected float ExtractFloat( int index)
        {
            return ExtractFloat(Data, index * 4);
        }

        protected float ToFloat(byte[] bytes, int index = 0)
        {
            return BitConverter.ToSingle(bytes, index);
        }

        protected short ToShort(byte[] bytes, int index = 0)
        {
            return BitConverter.ToInt16(bytes, index);
        }

        protected byte ToByte(byte[] bytes, int index = 0)
        {
            return bytes[index];
        }

        protected int ToInt(byte[] bytes, int index = 0)
        {
            return bytes[index];
        }

    }
}
