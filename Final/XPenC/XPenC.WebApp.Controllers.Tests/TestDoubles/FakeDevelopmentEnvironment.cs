namespace XPenC.WebApp.Controllers.Tests.TestDoubles
{
    public class FakeDevelopmentEnvironment : DummyWebHostEnvironment
    {
        public override string EnvironmentName => "Development";
    }
}