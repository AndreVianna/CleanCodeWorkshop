namespace XPenC.UI.Mvc.Tests.TestDoubles
{
    internal class FakeProductionEnvironment : DummyWebHostEnvironment
    {
        public override string EnvironmentName => "Production";
    }
}