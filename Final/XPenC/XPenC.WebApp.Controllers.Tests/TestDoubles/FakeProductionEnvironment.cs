namespace XPenC.WebApp.Controllers.Tests.TestDoubles
{
    internal class FakeProductionEnvironment : DummyWebHostEnvironment
    {
        public override string EnvironmentName => "Production";
    }
}