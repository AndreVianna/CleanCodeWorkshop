using Microsoft.AspNetCore.Mvc;
using TrdP.TestWebApp.Models.TestData;

namespace TrdP.TestWebApp.Models
{
    public class TestDataController : Controller
    {
        [HttpGet]
        public IActionResult Edit()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Edit(EditViewModel model)
        {
            return View(model);
        }
    }
}