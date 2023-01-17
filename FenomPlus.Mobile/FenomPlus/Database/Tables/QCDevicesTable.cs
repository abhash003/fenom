using System;
using System.Collections.Generic;
using System.Text;

namespace FenomPlus.Database.Tables
{
    public class QCDevicesTable : BaseTb<QCDevicesTable>
    {
        public string SerialNumber { get; set; }
        public string LastConnected { get; set; }
    }
}
