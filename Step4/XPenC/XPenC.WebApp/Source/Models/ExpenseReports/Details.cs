using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace XPenC.WebApp.Models.ExpenseReports
{
    public class Details : ListItem
    {
        public Details()
        {
            Items = new List<ExpenseReportItems.Details>();
        }

        [Display(Name = "Items")]
        public IList<ExpenseReportItems.Details> Items { get; set; }

        [Display(Name = "Meal Total")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal MealTotal { get; set; }

        [Display(Name = "Total")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Total { get; set; }
    }
}
