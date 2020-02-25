using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XPenC.WebApp.Controllers;
using Xunit;

namespace XPenC.UI.Mvc.Tests.Controllers
{
    public class HomeControllerTests
    {
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
            _controller = new HomeController
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
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

        [Fact]
        public void HomeController_SetLanguage_ShouldPass()
        {
            var result = _controller.SetLanguage("pt-BR", "/");

            Assert.IsType<LocalRedirectResult>(result);
        }
    }
}
