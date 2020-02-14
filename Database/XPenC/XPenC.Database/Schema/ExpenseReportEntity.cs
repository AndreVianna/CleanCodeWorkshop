using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace XPenC.Database.Schema
{
    [Table("ExpenseReports")]
    public class ExpenseReportEntity
    {
        public ExpenseReportEntity()
        {
            Items = new HashSet<ExpenseReportItemEntity>();
        }

        public int Id { get; set; }

        public string Client { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        [Column(TypeName = "decimal(8,4)")]
        public decimal MealTotal { get; set; }

        [Column(TypeName = "decimal(8,4)")]
        public decimal Total { get; set; }

        public ICollection<ExpenseReportItemEntity> Items { get; }
    }
}