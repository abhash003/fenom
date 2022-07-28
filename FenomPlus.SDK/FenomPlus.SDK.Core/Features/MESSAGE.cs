using System;
using System.Text;

namespace FenomPlus.SDK.Core.Features
{
    public class MESSAGE
    {
        public ushort IDMSG { get; set; }
        public ushort IDSUB { get; set; }
        public UInt64 IDVAR { get; set; }

        public MESSAGE(ID_MESSAGE _IDMSG, ID_SUB _IDSUB, UInt64 _IDVAR = 0)
        {
            IDMSG = (ushort)_IDMSG;
            IDSUB = (ushort)_IDSUB;
            IDVAR = _IDVAR;
        }

        public MESSAGE(ID_MESSAGE _IDMSG, ID_SUB _IDSUB, double _IDVAR)
        {
            IDMSG = (ushort)_IDMSG;
            IDSUB = (ushort)_IDSUB;
            IDVAR = (ulong)_IDVAR;
        }

        public MESSAGE(ID_MESSAGE _IDMSG, ID_SUB _IDSUB, Int16 val1, Int16 val2, Int16 val3, Int16 val4)
        {
            IDMSG = (ushort)_IDMSG;
            IDSUB = (ushort)_IDSUB;
            IDVAR = ((ulong)val1) << (16 + 16 + 16);
            IDVAR += ((ulong)val2) << (16 + 16);
            IDVAR += ((ulong)val3) << (16);
            IDVAR += ((ulong)val4);
        }

        public MESSAGE(ID_MESSAGE _IDMSG, ID_SUB _IDSUB, Int16 val1, Int16 val2, Int16 val3)
        {
            IDMSG = (ushort)_IDMSG;
            IDSUB = (ushort)_IDSUB;
            IDVAR = ((ulong)val1) << (16 + 16);
            IDVAR += ((ulong)val2) << (16);
            IDVAR += ((ulong)val3);
        }

        public MESSAGE(ID_MESSAGE _IDMSG, ID_SUB _IDSUB, Int16 val1, Int16 val2)
        {
            IDMSG = (ushort)_IDMSG;
            IDSUB = (ushort)_IDSUB;
            IDVAR += ((ulong)val1) << (16);
            IDVAR += ((ulong)val1);
        }

        public MESSAGE(ID_MESSAGE _IDMSG, ID_SUB _IDSUB, Int16 val1)
        {
            IDMSG = (ushort)_IDMSG;
            IDSUB = (ushort)_IDSUB;
            IDVAR += ((ulong)val1);
        }

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
                    IDVAR += ((ulong)data[index]);
                }
            }
        }

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
                    IDVAR += ((ulong)data[index]);
                }
            }
        }

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
                    IDVAR += ((ulong)(byte)Int32.Parse(dateTime.Substring(h, 2), System.Globalization.NumberStyles.HexNumber));
                }
            }
        }
    }
}
