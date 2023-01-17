using System;
using System.Collections.Generic;
using System.Text;

namespace FenomPlus.Database.Tables
{
    public class QualityControlTable : BaseTb<QualityControlTable>
    {
        public string DeviceSerialNumber { get; set; } = string.Empty;

        public string QCExpiration { get; set; } = string.Empty;

        public DateTime DateTaken { get; set; } = DateTime.Now;

        public string DeviceStatus { get; set; } = "Insufficient Data"; // Pass, Fail, Expired, Insufficient Data

        public List<QCUsersTable> Users { get; set; } = new List<QCUsersTable>();

    }
}
