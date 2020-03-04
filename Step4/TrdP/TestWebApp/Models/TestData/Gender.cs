using System.ComponentModel.DataAnnotations;

namespace TrdP.TestWebApp.Models.TestData
{
    public enum Gender
    {
        [Display(Name = "Not Informed")]NotInformed,
        Male,
        Female,
    }
}