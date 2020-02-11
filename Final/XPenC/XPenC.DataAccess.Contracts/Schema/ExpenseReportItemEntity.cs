using System;

namespace XPenC.DataAccess.Contracts.Schema
{
    public class ExpenseReportItemEntity
    {
        public int ExpenseReportId { get; set; }

        public int ItemNumber { get; set; }

        public DateTime? Date { get; set; }

        public string ExpenseType { get; set; }

        public string Description { get; set; }

        public decimal? Value { get; set; }
    }
}