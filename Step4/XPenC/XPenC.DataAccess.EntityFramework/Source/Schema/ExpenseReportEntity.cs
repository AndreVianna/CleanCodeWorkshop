using System;
using System.Collections.Generic;

namespace XPenC.DataAccess.EntityFramework.Schema
{
    public class ExpenseReportEntity
    {
        public ExpenseReportEntity()
        {
            Items = new HashSet<ExpenseReportItemEntity>();
        }

        public int Id { get; set; }
        public string Client { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public decimal MealTotal { get; set; }
        public decimal Total { get; set; }

        public ICollection<ExpenseReportItemEntity> Items { get; set; }
    }
}
