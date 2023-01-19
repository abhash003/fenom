using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace FenomPlus.Database.Tables
{
    public class QCDeviceTable : BaseTb<QCDeviceTable>
    {
        public string SerialNumber { get; set; }

        public QCNegativeControlTable NegativeControl { get; set; }

        public List<QCUserTable> Users { get; set; }

        public QCDeviceTable(string serialNumber)
        {
            // ToDo: Read from database

            SerialNumber = serialNumber;

            NegativeControl = new QCNegativeControlTable();
            Users = new List<QCUserTable>();
        }
    }
}
