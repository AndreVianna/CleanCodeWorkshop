using System.ComponentModel.DataAnnotations;

namespace XPenC.WebApp.Models
{
    public enum ExpenseType
    {
        [Display(Name = "Office")] Office,
        [Display(Name = "Meal")] Meal,
        [Display(Name = "Lodging (Hotel)")] HotelLodging,
        [Display(Name = "Lodging (Other)")] OtherLodging,
        [Display(Name = "Transportation (Land)")] LandTransportation,
        [Display(Name = "Transportation (Air)")] AirTransportation,
        [Display(Name = "Other")] Other,
    }
}