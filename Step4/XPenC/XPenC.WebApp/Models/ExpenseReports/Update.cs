using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace XPenC.WebApp.Models.ExpenseReports
{
    public class Update
    {
        public Update()
        {
            Items = new List<ExpenseReportItems.Details>();
        }

        public int Id { get; set; }

        [Required]
        [Display(Name = "Client")]
        public string Client { get; set; }

        public IList<ExpenseReportItems.Details> Items { get; set; }
    }
}