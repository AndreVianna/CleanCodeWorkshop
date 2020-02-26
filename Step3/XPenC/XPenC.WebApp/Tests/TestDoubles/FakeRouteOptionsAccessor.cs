using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace XPenC.WebApp.Tests.TestDoubles
{
    internal class FakeRouteOptionsAccessor : IOptions<RouteOptions>
    {
        public RouteOptions Value { get; } = new RouteOptions();
    }
}