using System;
using System.Collections.Generic;

namespace XPenC.WebApp.Controllers.ViewModels
{
    public class ExpenseReportDetails
    {
        public ExpenseReportDetails()
        {
            Items = new List<ExpenseReportItemDetails>();
        }

        public int Id { get; set; }

        public string Client { get; set; }

        public DateTime CreatedOn { get; set; }
        
        public DateTime ModifiedOn { get; set; }

        public IList<ExpenseReportItemDetails> Items { get; set; }
        
        public decimal MealTotal { get; set; }

        public decimal Total { get; set; }
    }
}
