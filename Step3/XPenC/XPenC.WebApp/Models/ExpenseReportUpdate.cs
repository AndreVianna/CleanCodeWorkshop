using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace XPenC.WebApp.Models
{
    public class ExpenseReportUpdate
    {
        public ExpenseReportUpdate()
        {
            DisplayItems = new List<ExpenseReportItemDetails>();
            NewItem = new ExpenseReportItemUpdate();
        }

        public int Id { get; set; }

        [Required]
        [Display(Name = "Client")]
        public string Client { get; set; }

        public IList<ExpenseReportItemDetails> DisplayItems { get; set; }

        public ExpenseReportItemUpdate NewItem { get; set; }
    }
}