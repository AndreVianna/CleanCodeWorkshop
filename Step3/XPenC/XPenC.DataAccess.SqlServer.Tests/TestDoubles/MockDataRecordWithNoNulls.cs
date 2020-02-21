using System;

namespace XPenC.DataAccess.SqlServer.Tests.TestDoubles
{
    internal class MockDataRecordWithNoNulls : DummyDataRecord
    {
        public MockDataRecordWithNoNulls(int fieldCount)
        {
            FieldCount = fieldCount;
        }

        public override int FieldCount { get; }

        public override int GetOrdinal(string _) => 1;
        public override bool IsDBNull(int _) => false;
        public override int GetInt32(int _) => 1;
        public override DateTime GetDateTime(int _) => DateTime.Now.AddMinutes(-5);
        public override decimal GetDecimal(int _) => 1;
        public override string GetString(int _) => "string";

    }
}