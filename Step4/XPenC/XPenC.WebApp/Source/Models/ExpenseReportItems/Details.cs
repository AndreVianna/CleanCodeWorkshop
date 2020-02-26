using System;
using System.ComponentModel.DataAnnotations;

namespace XPenC.WebApp.Models.ExpenseReportItems
{
    public class Details
    {
        [Display(Name = "Number")]
        public int Number { get; set; }

        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:d}", NullDisplayText = "[Pending]")]
        public DateTime? Date { get; set; }

        [Display(Name = "Type")]
        public ExpenseType? ExpenseType { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Value")]
        [DisplayFormat(DataFormatString = "{0:C}", NullDisplayText = "[Pending]")]
        public decimal? Value { get; set; }

        public bool IsAboveMaximum { get; set; }
    }
}