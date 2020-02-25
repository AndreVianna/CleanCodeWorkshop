namespace XPenC.UI.Mvc.Tests.TestDoubles
{
    internal class FakeDevelopmentEnvironment : DummyWebHostEnvironment
    {
        public override string EnvironmentName => "Development";
    }
}