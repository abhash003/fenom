using System;
using System.Collections.Generic;
using System.Text;

namespace FenomPlus.Database.Tables
{
    public class QCUsersTable : BaseTb<QCUsersTable>
    {
        public string Name { get; set; }  // User name

        //•	Conditionally Qualified
        //      - Fewer than 4 tests have been performed by the QC User.
        //      - All tests within the Qualification Period are “Pass”.
        //•	Qualified
        //      - Latest test is within the expected range for the QC User.
        //•	Disqualified
        //      - Latest test is outside the expected range for the QC User.
        //•	None
        //      - QC User test is required.


        public string CurrentStatus { get; set; } // Conditionally Qualified, Qualified, Disqualified, None

        public DateTime DateAdded { get; set; }

        public DateTime DateCreated { get; set; }  // Date created

        public DateTime ExpiresDate { get; set; }  // Date in which this expires?

        public DateTime NextTestDate { get; set; }  // Next Test date?

        public List<QCResultsTable> TestResults;

        // Use TestResults as Chart Data

        public QCUsersTable(string userName)
        {
            Name = userName;
            DateAdded = DateTime.Now;
            CurrentStatus = "Conditionally Qualified"; // When new user has no tests
            TestResults = new List<QCResultsTable>();
        }

        private string GetStatus()
        {

        }
 

    }
}
