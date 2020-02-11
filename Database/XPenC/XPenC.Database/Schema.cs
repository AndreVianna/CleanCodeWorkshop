using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace XPenC.Database
{
    public class ExpenseReport
    {
        public ExpenseReport()
        {
            Items = new HashSet<ExpenseReportItem>();
        }

        public int Id { get; set; }

        public string Client { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        [Column(TypeName = "decimal(8,4)")]
        public decimal MealTotal { get; set; }

        [Column(TypeName = "decimal(8,4)")]
        public decimal Total { get; set; }

        public ICollection<ExpenseReportItem> Items { get; }
    }

    public class ExpenseReportItem
    {
        public int ExpenseReportId { get; set; }
        public ExpenseReport ExpenseReport { get; set; }

        public int ItemNumber { get; set; }

        public DateTime? Date { get; set; }

        public string ExpenseType { get; set; }

        public string Description { get; set; }

        [Column(TypeName = "decimal(8,4)")]
        public decimal? Value { get; set; }
    }
}