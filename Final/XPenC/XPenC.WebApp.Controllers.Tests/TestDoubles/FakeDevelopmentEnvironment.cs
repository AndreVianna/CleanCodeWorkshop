namespace XPenC.WebApp.Controllers.Tests.TestDoubles
{
    internal class FakeDevelopmentEnvironment : DummyWebHostEnvironment
    {
        public override string EnvironmentName => "Development";
    }
}