using System;
using System.Text;

namespace FenomPlus.SDK.Core.Features
{
    public class MESSAGE
    {
        public ushort IDMSG { get; set; }
        public ushort IDSUB { get; set; }
        public byte[] IDVAR { get; set; }

        /// </summary>
        /// <param name="_IDMSG"></param>
        /// <param name="_IDSUB"></param>
        /// <param name="val"></param>
        /// device info
        public MESSAGE(ID_MESSAGE _IDMSG, ID_SUB _IDSUB, Int64 val = 0)
        {
            IDMSG = (ushort)_IDMSG;
            IDSUB = (ushort)_IDSUB;

            IDVAR = new byte[8];            // If IDVAR is NULL App crashes

            BitConverter.GetBytes(val).CopyTo(IDVAR, 0);
        }

        public MESSAGE(ID_MESSAGE _IDMSG, ID_SUB _IDSUB, Int32 val)
        {
            IDMSG = (ushort)_IDMSG;
            IDSUB = (ushort)_IDSUB;
            IDVAR = new byte[4];            // If IDVAR is NULL App crashes

            BitConverter.GetBytes(val).CopyTo(IDVAR, 0);
        }
        public MESSAGE(ID_MESSAGE _IDMSG, ID_SUB _IDSUB, Int16 val)
        {
            IDMSG = (ushort)_IDMSG;
            IDSUB = (ushort)_IDSUB;

            IDVAR = new byte[2];            // If IDVAR is NULL App crashes
            BitConverter.GetBytes(val).CopyTo(IDVAR, 0);
        }
        public MESSAGE(ID_MESSAGE _IDMSG, ID_SUB _IDSUB, UInt16 val)
        {
            IDMSG = (ushort)_IDMSG;
            IDSUB = (ushort)_IDSUB;

            IDVAR = new byte[2];            // If IDVAR is NULL App crashes
            BitConverter.GetBytes(val).CopyTo(IDVAR, 0);
        }

        public MESSAGE(ID_MESSAGE _IDMSG, ID_SUB _IDSUB, Byte val)
        {
            IDMSG = (ushort)_IDMSG;
            IDSUB = (ushort)_IDSUB;

            IDVAR = new byte[1];            // If IDVAR is NULL App crashes

            IDVAR[0] = val;
        }
 
        /// </summary>
        /// <param name="_IDMSG"></param>
        /// <param name="_IDSUB"></param>
        /// <param name="val1"></param>
        /// <param name="val2"></param>
        /// <param name="val3"></param>
        /// calibration data
        public MESSAGE(ID_MESSAGE _IDMSG, ID_SUB _IDSUB, double val1, double val2, double val3)
        {
            IDMSG = (ushort)_IDMSG;
            IDSUB = (ushort)_IDSUB;

            IDVAR = new byte[24];

            byte[] value1 = new byte[8];
            byte[] value2 = new byte[8];
            byte[] value3 = new byte[8];

            value1 = BitConverter.GetBytes(val1);
            value2 = BitConverter.GetBytes(val2);
            value3 = BitConverter.GetBytes(val3);

            Buffer.BlockCopy(value1, 0, IDVAR, 0, 8);
            Buffer.BlockCopy(value2, 0, IDVAR, 8, 8);
            Buffer.BlockCopy(value3, 0, IDVAR, 16, 8);

        }

        public MESSAGE(ID_MESSAGE _IDMSG, ID_SUB _IDSUB, float val)
        {
            IDMSG = (ushort)_IDMSG;
            IDSUB = (ushort)_IDSUB;
            //IDVAR = (Int64)val;
        }

        public MESSAGE(ID_MESSAGE _IDMSG, ID_SUB _IDSUB, Int32 val1, Int32 val2)
        {
            IDMSG  = (ushort)_IDMSG;
            IDSUB  = (ushort)_IDSUB;
            //IDVAR  = ((Int64)val1) << (32);
            //IDVAR += ((Int64)val2);
        }

        public MESSAGE(ID_MESSAGE _IDMSG, ID_SUB _IDSUB, float val1, float val2)
        {
            IDMSG  = (ushort)_IDMSG;
            IDSUB  = (ushort)_IDSUB;
            //IDVAR  = ((Int64)val1) << (32);
            //IDVAR += ((Int64)val2);
        }

        public MESSAGE(ID_MESSAGE _IDMSG, ID_SUB _IDSUB, Int16 val1, Int16 val2, Int16 val3, Int16 val4)
        {
            IDMSG  = (ushort)_IDMSG;
            IDSUB  = (ushort)_IDSUB;
            //IDVAR  = ((Int64)val1) << (48);
            //IDVAR += ((Int64)val2) << (32);
            //IDVAR += ((Int64)val3) << (16);
            //IDVAR += ((Int64)val4);
        }

        public MESSAGE(ID_MESSAGE _IDMSG, ID_SUB _IDSUB, Int16 val1, Int16 val2, Int16 val3)
        {
            IDMSG  = (ushort)_IDMSG;
            IDSUB  = (ushort)_IDSUB;
            //IDVAR  = ((Int64)val1) << (32);
            //IDVAR += ((Int64)val2) << (16);
            //IDVAR += ((Int64)val3);
        }

        public MESSAGE(ID_MESSAGE _IDMSG, ID_SUB _IDSUB, Int16 val1, Int16 val2)
        {
            IDMSG  = (ushort)_IDMSG;
            IDSUB  = (ushort)_IDSUB;
            //IDVAR  = ((Int64)val1) << (16);
           // IDVAR += ((Int64)val2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_IDMSG"></param>
        /// <param name="_IDSUB"></param>
        /// <param name="_IDVAR"></param>
        public MESSAGE(ID_MESSAGE _IDMSG, ID_SUB _IDSUB, byte[] _IDVAR)
        {
            IDMSG = (ushort)_IDMSG;
            IDSUB = (ushort)_IDSUB;

            byte[] data = _IDVAR;
            if ((data != null) && (data.Length > 0))
            {
                // copy array from lsb and shift 8 bytes to the left until max 8 bytes or less
                for (int index = 0; (index < data.Length) && (index < 8); index++)
                {
                    // shift first
                   // IDVAR <<= 8;
                    // add to lsb
                   // IDVAR += ((Int64)data[index]);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_IDMSG"></param>
        /// <param name="_IDSUB"></param>
        /// <param name="_IDVAR"></param>
        /// serial number
        public MESSAGE(ID_MESSAGE _IDMSG, ID_SUB _IDSUB, string _IDVAR)
        {
            IDMSG = (ushort)_IDMSG;
            IDSUB = (ushort)_IDSUB;

            IDVAR = Encoding.ASCII.GetBytes(_IDVAR);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_IDMSG"></param>
        /// <param name="_IDSUB"></param>
        /// <param name="_IDVAR"></param>
        /// date and time
        public MESSAGE(ID_MESSAGE _IDMSG, ID_SUB _IDSUB, DateTime _IDVAR)
        {
            //string dateTime = _IDVAR.ToString("MMddyyyyHHmmss");  Older Value
            string dateTime = _IDVAR.ToString("yyyyMMddTHH:mm:ss");
            IDMSG = (ushort)_IDMSG;
            IDSUB = (ushort)_IDSUB;

            IDVAR = Encoding.ASCII.GetBytes(dateTime);
        }
    }
}
