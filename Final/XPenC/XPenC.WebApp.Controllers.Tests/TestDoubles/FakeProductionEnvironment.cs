namespace XPenC.WebApp.Controllers.Tests.TestDoubles
{
    public class FakeProductionEnvironment : DummyWebHostEnvironment
    {
        public override string EnvironmentName => "Production";
    }
}