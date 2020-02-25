namespace XPenC.Common.TestDoubles
{
    public class FakeConfiguration : DummyConfiguration
    {
        public override string this[string key] => "SomeValue";
    }
}