using System;
using System.Collections.Generic;
using System.Text;

namespace FenomPlus.Database.Tables
{
    public class QCNegativeControlTable : BaseTb<QCNegativeControlTable>
    {
        public string Name { get; set; }

        public string CurrentStatus { get; set; } // Pass, Fail, Expired

        public DateTime DateCreated { get; set; }  // Date created

        public DateTime ExpiresDate { get; set; }  // Date in which this expires?

        public DateTime NextTestDate { get; set; }  // Next Test date?

        public float[] ChartData { get; set; } = new float[4];


        public QCNegativeControlTable()
        {
            Name = "Negative Control";
            CurrentStatus = "None";
            DateCreated = DateTime.Now;
            ExpiresDate = DateCreated.AddHours(16); // 
            NextTestDate = DateCreated.AddHours(16);
        }
 

    }
}
