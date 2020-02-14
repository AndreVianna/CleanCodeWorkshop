using System.Collections.Generic;

namespace XPenC.WebApp.Controllers.ViewModels
{
    public class ExpenseReportUpdate
    {
        public ExpenseReportUpdate()
        {
            DisplayItems = new List<ExpenseReportItemDisplay>();
            NewItem = new ExpenseReportItemUpdate();
        }

        public int Id { get; set; }

        public string Client { get; set; }

        public IList<ExpenseReportItemDisplay> DisplayItems { get; set; }

        public ExpenseReportItemUpdate NewItem { get; set; }

        public string AddActionName { get; set; }
        public string SaveActionName { get; set; }
        public string FinishActionName { get; set; }
    }
}