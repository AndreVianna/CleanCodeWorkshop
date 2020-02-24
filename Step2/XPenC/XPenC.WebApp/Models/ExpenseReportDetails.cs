using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace XPenC.WebApp.Models
{
    public class ExpenseReportDetails : ExpenseReportListItem
    {
        public ExpenseReportDetails()
        {
            Items = new List<ExpenseReportItemDetails>();
        }

        [Display(Name = "Items")]
        public IList<ExpenseReportItemDetails> Items { get; set; }

        [Display(Name = "Meal Total")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal MealTotal { get; set; }

        [Display(Name = "Total")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Total { get; set; }
    }
}
