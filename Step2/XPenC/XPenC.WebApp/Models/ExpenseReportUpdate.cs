using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace XPenC.WebApp.Models
{
    public class ExpenseReportUpdate
    {
        public ExpenseReportUpdate()
        {
            Items = new List<ExpenseReportItemDetails>();
        }

        public int Id { get; set; }

        [Required]
        [Display(Name = "Client")]
        public string Client { get; set; }

        public IList<ExpenseReportItemDetails> Items { get; set; }
    }
}