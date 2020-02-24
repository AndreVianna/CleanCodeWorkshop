using System;
using System.ComponentModel.DataAnnotations;

namespace XPenC.WebApp.Models.ExpenseReports
{
    public class ExpenseReportListItem
    {
        public int Id { get; set; }

        [Display(Name = "Client")]
        public string Client { get; set; }

        [Display(Name = "Created On")]
        public DateTime CreatedOn { get; set; }

        [Display(Name = "Modified On")]
        public DateTime ModifiedOn { get; set; }
    }
}