using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace TrdP.TestWebApp.Models.TestData
{
    public class EditViewModel
    {
        [HiddenInput]
        public int Id { get; set; }

        [Required]
        [StringLength(32, MinimumLength = 3)]
        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [Phone]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Compare("Email")]
        [DisplayName("Email Confirmation")]
        public string EmailConfirmation { get; set; }

        [CreditCard]
        [Display(Name = "Credit Card")]
        public string CreditCard { get; set; }

        [Url]
        [Display(Name = "LinkedIn Profile")]
        public string LinkedIn { get; set; }

        [Range(18, 80)]
        public int Age { get; set; }

        public Gender Gender { get; set; }
    }
}
