using System;
using System.Runtime.InteropServices;

namespace FenomPlus.SDK.Core.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class DebugMsg : BaseCharacteristic
    {
        public static int Min = 1;

        public string Message { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DebugMsg Create(byte[] data)
        {
            DebugMsg debugMsg = new DebugMsg();
            return debugMsg.Decode(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public DebugMsg Decode(byte[] data)
        {
            try
            {
                Data = data;
                //if ((data != null) && (data.Length >= Min))
                {
                    Message = BitConverter.ToString(data);
                }
            } finally { }
            return this;
        }

    }
}
