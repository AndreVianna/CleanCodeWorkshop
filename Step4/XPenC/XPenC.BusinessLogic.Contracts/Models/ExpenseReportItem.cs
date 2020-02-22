using System;

namespace XPenC.BusinessLogic.Contracts.Models
{
    public class ExpenseReportItem
    {
        public int ExpenseReportId { get; set; }

        public int ItemNumber { get; set; }

        public DateTime Date { get; set; }

        public ExpenseType ExpenseType { get; set; }

        public string Description { get; set; }

        public decimal Value { get; set; }

        public bool IsAboveMaximum { get; set; }
    }
}