using System.Collections.Generic;

namespace XPenC.WebApp.ViewModels
{
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
}