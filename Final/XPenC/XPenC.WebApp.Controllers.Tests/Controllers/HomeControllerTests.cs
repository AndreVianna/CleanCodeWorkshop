using Microsoft.AspNetCore.Mvc;
using XPenC.WebApp.Controllers.Controllers;
using Xunit;

namespace XPenC.WebApp.Controllers.Tests.Controllers
{
    public class HomeControllerTests
    {
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
            _controller = new HomeController();
        }

        [Fact]
        public void HomeController_Index_ShouldPass()
        {
            var result = _controller.Index();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void HomeController_Privacy_ShouldPass()
        {
            var result = _controller.Privacy();

            Assert.IsType<ViewResult>(result);
        }
    }
}