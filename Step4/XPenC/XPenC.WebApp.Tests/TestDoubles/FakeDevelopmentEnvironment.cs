namespace XPenC.WebApp.Tests.TestDoubles
{
    internal class FakeDevelopmentEnvironment : DummyWebHostEnvironment
    {
        public override string EnvironmentName => "Development";
    }
}