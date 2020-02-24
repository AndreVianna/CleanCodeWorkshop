using System;
using System.ComponentModel.DataAnnotations;

namespace XPenC.WebApp.Models.ExpenseReports
{
    public class ExpenseReportItemUpdate
    {
        public ExpenseReportItemUpdate()
        {
            Date = DateTime.Now;
        }

        public int ExpenseReportId { get; set; }
        public ExpenseReportDetails ExpenseReport { get; set; }

        [Required]
        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime Date { get; set; }

        [Required]
        [Display(Name = "Type")]
        public ExpenseType ExpenseType { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Value")]
        public decimal Value { get; set; }
    }
}