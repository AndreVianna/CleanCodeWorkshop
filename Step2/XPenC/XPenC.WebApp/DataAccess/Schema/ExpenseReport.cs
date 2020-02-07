using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace XPenC.WebApp.DataAccess.Schema
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
}