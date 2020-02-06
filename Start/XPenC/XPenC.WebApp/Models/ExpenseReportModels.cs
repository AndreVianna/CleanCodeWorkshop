using System;

namespace XPenC.WebApp.Models
{
    public class ExpenseReportDetails
    {
        public int Id { get; set; }

        public string Client { get; set; }

        public DateTime CreatedOn { get; set; }
        
        public DateTime ModifiedOn { get; set; }

        public ExpenseReportItem[] Items { get; set; }
        
        public decimal MealTotal { get; set; }

        public decimal Total { get; set; }

        public class ExpenseReportItem
        {
            public int? Number { get; set; }

            public DateTime? Date { get; set; }

            public string ExpenseType { get; set; }

            public string Description { get; set; }

            public decimal? Value { get; set; }
        }
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
            Items = Array.Empty<ExpenseReportItem>();
            NewItem = new ExpenseReportItem();
        }

        public int Id { get; set; }

        public string Client { get; set; }

        public ExpenseReportItem[] Items { get; set; }

        public ExpenseReportItem NewItem { get; set; }

        public class ExpenseReportItem
        {
            public ExpenseReportItem()
            {
                Date = DateTime.Now;
                Value = 0;
            }

            public int? Number { get; set; }

            public DateTime? Date { get; set; }

            public string ExpenseType { get; set; }

            public string Description { get; set; }

            public decimal? Value { get; set; }
        }
    }
}
