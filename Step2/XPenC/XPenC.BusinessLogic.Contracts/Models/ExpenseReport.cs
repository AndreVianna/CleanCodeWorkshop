using System;
using System.Collections.Generic;

namespace XPenC.BusinessLogic.Contracts.Models
{
    public class ExpenseReport
    {
        public ExpenseReport()
        {
            Items = new List<ExpenseReportItem>();
        }

        public int Id { get; set; }

        public string Client { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public decimal MealTotal { get; set; }

        public decimal Total { get; set; }

        public ICollection<ExpenseReportItem> Items { get; set; }
    }
}