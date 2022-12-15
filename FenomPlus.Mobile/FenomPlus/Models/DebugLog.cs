using System;
using System.Globalization;
using System.Text;

namespace FenomPlus.Models
{
    public class DebugLog
    {
        public DateTime DateTime { get; set; }
        public string FormattedDateTime => DateTime.ToString("yyyymmdd HH:mm:ss:ffff");
        public string Msg { get; set; }
        public string HexMsg { get; set; }
        public byte[] RawMsg { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rawMsg"></param>
        public DebugLog(byte[] rawMsg)
        {
            this.RawMsg = rawMsg;
            this.DateTime = DateTime.Now;
            this.HexMsg = BitConverter.ToString(RawMsg);
            this.Msg = Encoding.UTF8.GetString(RawMsg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rawMsg"></param>
        public DebugLog(string rawMsg)
        {
            this.RawMsg = Encoding.Default.GetBytes(rawMsg);
            this.DateTime = DateTime.Now;
            this.HexMsg = BitConverter.ToString(RawMsg);
            this.Msg = Encoding.UTF8.GetString(RawMsg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rawMsg"></param>
        /// <returns></returns>
        public static DebugLog Create(byte[] rawMsg)
        {
            return new DebugLog(rawMsg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static DebugLog Create(string msg)
        {
            return new DebugLog(msg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string content = string.Format("{0}\n{1}\n{2}\n",
                DateTime.ToString(Constants.DateTimeFormatString, CultureInfo.InvariantCulture),
                HexMsg,
                Msg);
            return content;
        }
    }
}
