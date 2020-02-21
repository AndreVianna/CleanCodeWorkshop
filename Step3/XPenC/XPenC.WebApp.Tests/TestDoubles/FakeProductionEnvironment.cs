namespace XPenC.WebApp.Tests.TestDoubles
{
    internal class FakeProductionEnvironment : DummyWebHostEnvironment
    {
        public override string EnvironmentName => "Production";
    }
}