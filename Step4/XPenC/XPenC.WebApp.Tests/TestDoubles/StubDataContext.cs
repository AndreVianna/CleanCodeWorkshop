namespace XPenC.UI.Mvc.Tests.TestDoubles
{
    internal class StubDataContext : DummyDataContext
    {
        public override void CommitChanges()
        {
        }
    }
}