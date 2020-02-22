namespace XPenC.WebApp.Tests.TestDoubles
{
    internal class FakeConfiguration : DummyConfiguration
    {
        public override string this[string key] => "SomeValue";
    }
}