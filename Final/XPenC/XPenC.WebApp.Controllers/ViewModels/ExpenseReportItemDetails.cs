using System;

namespace XPenC.WebApp.ViewModels
{
    public class ExpenseReportItemDetails
    {
        public ExpenseReportItemDetails()
        {
            Date = DateTime.Now;
            Value = 0;
        }

        public int Number { get; set; }

        public DateTime? Date { get; set; }

        public string ExpenseType { get; set; }

        public string Description { get; set; }

        public decimal? Value { get; set; }
    }
}