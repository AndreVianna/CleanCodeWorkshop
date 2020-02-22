namespace XPenC.DataAccess.SqlServer.Tests.TestDoubles
{
    internal class MockDataRecordWithNulls : MockDataRecordWithNoNulls
    {
        public MockDataRecordWithNulls(int fieldCount) : base(fieldCount)
        {
        }

        public override bool IsDBNull(int _) => true;
    }
}