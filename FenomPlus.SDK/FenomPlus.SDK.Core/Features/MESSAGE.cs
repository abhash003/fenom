using System;
using System.Text;

namespace FenomPlus.SDK.Core.Features
{
    public class MESSAGE
    {
        public ushort IDMSG { get; set; }
        public ushort IDSUB { get; set; }
        public Int64 IDVAR { get; set; }

        public MESSAGE(ID_MESSAGE _IDMSG, ID_SUB _IDSUB, Int64 val = 0)
        {
            IDMSG = (ushort)_IDMSG;
            IDSUB = (ushort)_IDSUB;
            IDVAR = val;
        }

        public MESSAGE(ID_MESSAGE _IDMSG, ID_SUB _IDSUB, Int32 val)
        {
            IDMSG = (ushort)_IDMSG;
            IDSUB = (ushort)_IDSUB;
            IDVAR = val;
        }

        public MESSAGE(ID_MESSAGE _IDMSG, ID_SUB _IDSUB, Int16 val)
        {
            IDMSG = (ushort)_IDMSG;
            IDSUB = (ushort)_IDSUB;
            IDVAR = val;
        }

        public MESSAGE(ID_MESSAGE _IDMSG, ID_SUB _IDSUB, double val)
        {
            IDMSG = (ushort)_IDMSG;
            IDSUB = (ushort)_IDSUB;
            IDVAR = (Int64)val;
        }

        public MESSAGE(ID_MESSAGE _IDMSG, ID_SUB _IDSUB, float val)
        {
            IDMSG = (ushort)_IDMSG;
            IDSUB = (ushort)_IDSUB;
            IDVAR = (Int64)val;
        }

        public MESSAGE(ID_MESSAGE _IDMSG, ID_SUB _IDSUB, Int32 val1, Int32 val2)
        {
            IDMSG  = (ushort)_IDMSG;
            IDSUB  = (ushort)_IDSUB;
            IDVAR  = ((Int64)val1) << (32);
            IDVAR += ((Int64)val2);
        }

        public MESSAGE(ID_MESSAGE _IDMSG, ID_SUB _IDSUB, float val1, float val2)
        {
            IDMSG  = (ushort)_IDMSG;
            IDSUB  = (ushort)_IDSUB;
            IDVAR  = ((Int64)val1) << (32);
            IDVAR += ((Int64)val2);
        }

        public MESSAGE(ID_MESSAGE _IDMSG, ID_SUB _IDSUB, Int16 val1, Int16 val2, Int16 val3, Int16 val4)
        {
            IDMSG  = (ushort)_IDMSG;
            IDSUB  = (ushort)_IDSUB;
            IDVAR  = ((Int64)val1) << (48);
            IDVAR += ((Int64)val2) << (32);
            IDVAR += ((Int64)val3) << (16);
            IDVAR += ((Int64)val4);
        }

        public MESSAGE(ID_MESSAGE _IDMSG, ID_SUB _IDSUB, Int16 val1, Int16 val2, Int16 val3)
        {
            IDMSG  = (ushort)_IDMSG;
            IDSUB  = (ushort)_IDSUB;
            IDVAR  = ((Int64)val1) << (32);
            IDVAR += ((Int64)val2) << (16);
            IDVAR += ((Int64)val3);
        }

        public MESSAGE(ID_MESSAGE _IDMSG, ID_SUB _IDSUB, Int16 val1, Int16 val2)
        {
            IDMSG  = (ushort)_IDMSG;
            IDSUB  = (ushort)_IDSUB;
            IDVAR  = ((Int64)val1) << (16);
            IDVAR += ((Int64)val2);
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
                    IDVAR <<= 8;
                    // add to lsb
                    IDVAR += ((Int64)data[index]);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_IDMSG"></param>
        /// <param name="_IDSUB"></param>
        /// <param name="_IDVAR"></param>
        public MESSAGE(ID_MESSAGE _IDMSG, ID_SUB _IDSUB, string _IDVAR)
        {
            IDMSG = (ushort)_IDMSG;
            IDSUB = (ushort)_IDSUB;

            byte[] data = Encoding.ASCII.GetBytes(_IDVAR);
            if ((data != null) && (data.Length > 0))
            {
                // copy array from lsb and shift 8 bytes to the left until max 8 bytes or less
                for (int index = 0; (index < data.Length) && (index < 8); index++)
                {
                    // shift first
                    IDVAR <<= 8;
                    // add to lsb
                    IDVAR += ((Int64)data[index]);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_IDMSG"></param>
        /// <param name="_IDSUB"></param>
        /// <param name="_IDVAR"></param>
        public MESSAGE(ID_MESSAGE _IDMSG, ID_SUB _IDSUB, DateTime _IDVAR)
        {
            string dateTime = _IDVAR.ToString("MMddyyyyHHmmss");
            IDMSG = (ushort)_IDMSG;
            IDSUB = (ushort)_IDSUB;

            byte[] data = new byte[dateTime.Length / 2];
            if ((data != null) && (data.Length > 0))
            {
                // copy array from lsb and shift 8 bytes to the left until max 8 bytes or less
                for (int index = 0, h =0; (index < data.Length) && (index < 8); index++, h +=2)
                {
                    // shift first
                    IDVAR <<= 8;
                    // add to lsb
                    IDVAR += ((Int64)(byte)Int32.Parse(dateTime.Substring(h, 2), System.Globalization.NumberStyles.HexNumber));
                }
            }
        }
    }
}
