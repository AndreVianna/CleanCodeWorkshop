using System;

namespace XPenC.WebApp.Controllers.ViewModels
{
    public class ExpenseReportItemDetails
    {
        public int Number { get; set; }

        public DateTime? Date { get; set; }

        public string ExpenseType { get; set; }

        public string Description { get; set; }

        public decimal? Value { get; set; }

        public bool IsAboveMaximum { get; set; }
    }
}