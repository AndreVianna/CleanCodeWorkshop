using System;

namespace XPenC.WebApp.ViewModels
{
    public class ExpenseReportListItem
    {
        public int Id { get; set; }

        public string Client { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }
    }
}