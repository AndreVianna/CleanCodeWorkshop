using System;
using System.Collections.Generic;

namespace XPenC.WebApp.Models
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

    public class ExpenseReportListItem
    {
        public int Id { get; set; }

        public string Client { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }
    }

    public class ExpenseReportUpdateInput
    {
        public ExpenseReportUpdateInput()
        {
            Items = new List<ExpenseReportItemDetails>();
            NewItem = new ExpenseReportItemDetails();
        }

        public int Id { get; set; }

        public string Client { get; set; }

        public IList<ExpenseReportItemDetails> Items { get; set; }

        public ExpenseReportItemDetails NewItem { get; set; }
    }

    public class ExpenseReportItemDetails
    {
        public ExpenseReportItemDetails()
        {
            Date = DateTime.Now;
            Value = 0;
        }

        public int Number { get; set; }

        public DateTime? Date { get; set; }

        public string ExpenseType { get; set; }

        public string Description { get; set; }

        public decimal? Value { get; set; }
    }
}
