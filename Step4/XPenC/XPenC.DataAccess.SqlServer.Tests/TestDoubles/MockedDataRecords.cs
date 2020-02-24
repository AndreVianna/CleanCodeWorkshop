using System;

namespace XPenC.DataAccess.SqlServer.Tests.TestDoubles
{
    internal class IntegerDataRecord : DummyDataRecord
    {
        public override int FieldCount => 1;

        public override int GetOrdinal(string _) => 1;
        public override bool IsDBNull(int _) => false;
        public override int GetInt32(int _) => 1;
    }

    internal class ExpenseReportDataRecord : DummyDataRecord
    {
        public override int FieldCount => 6;

        public override int GetOrdinal(string _) => 1;
        public override bool IsDBNull(int _) => false;
        public override int GetInt32(int _) => 1;
        public override DateTime GetDateTime(int _) => DateTime.Now.AddMinutes(-5);
        public override decimal GetDecimal(int _) => 1;
        public override string GetString(int _) => "";
    }

    internal class ExpenseReportDataRecordWithNulls : ExpenseReportDataRecord
    {
        public override bool IsDBNull(int _) => true;
    }

    internal class ExpenseReportItemDataRecord : ExpenseReportDataRecord
    {
        private readonly string _expenseType;

        public ExpenseReportItemDataRecord(string expenseType)
        {
            _expenseType = expenseType;
        }

        public override string GetString(int _) => _expenseType;
    }

    internal class ExpenseReportItemDataRecordWithNulls : ExpenseReportItemDataRecord
    {
        public ExpenseReportItemDataRecordWithNulls(string expenseType) : base(expenseType)
        {
        }

        public override bool IsDBNull(int _) => true;
    }

    internal class ExpenseReportDataJoinRecord : ExpenseReportItemDataRecord
    {
        public ExpenseReportDataJoinRecord(string expenseType) : base(expenseType)
        {
        }

        public override int FieldCount => 12;
    }
}