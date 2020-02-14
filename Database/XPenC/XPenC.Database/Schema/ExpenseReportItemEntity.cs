using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace XPenC.Database.Schema
{
    [Table("ExpenseReportItems")]
    public class ExpenseReportItemEntity
    {
        public int ExpenseReportId { get; set; }
        public ExpenseReportEntity ExpenseReport { get; set; }

        public int ItemNumber { get; set; }

        public DateTime? Date { get; set; }

        public string ExpenseType { get; set; }

        public string Description { get; set; }

        [Column(TypeName = "decimal(8,4)")]
        public decimal? Value { get; set; }
    }
}