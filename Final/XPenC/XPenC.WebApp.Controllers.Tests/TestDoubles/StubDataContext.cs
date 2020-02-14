namespace XPenC.WebApp.Controllers.Tests.TestDoubles
{
    internal class StubDataContext : DummyDataContext
    {
        public override void CommitChanges()
        {
        }
    }
}