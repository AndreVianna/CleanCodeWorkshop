namespace XPenC.WebApp.Controllers.Tests.TestDoubles
{
    public class StubDataContext : DummyDataContext
    {
        public override void CommitChanges()
        {
        }
    }
}