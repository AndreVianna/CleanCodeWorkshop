using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;

namespace TrdP.Common.TestDoubles
{
    public class FakeMvcBuilder : IMvcBuilder
    {
        public IServiceCollection Services { get; } = new FakeServiceCollection();
        public ApplicationPartManager PartManager => new ApplicationPartManager();
    }
}