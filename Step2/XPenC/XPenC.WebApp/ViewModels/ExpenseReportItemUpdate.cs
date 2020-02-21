using System;

namespace XPenC.WebApp.ViewModels
{
    public class ExpenseReportItemUpdate
    {
        public ExpenseReportItemUpdate()
        {
            Date = DateTime.Now;
            Value = 0;
        }

        public DateTime? Date { get; set; }

        public string ExpenseType { get; set; }

        public string Description { get; set; }

        public decimal? Value { get; set; }
    }
}