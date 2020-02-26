using System;
using System.Collections.Generic;
using System.Data;

namespace XPenC.DataAccess.SqlServer.Native.Tests.TestDoubles
{
    internal class MockSqlDataProviderWithInvalidValues : MockSqlDataProvider
    {
        public override IEnumerable<T> ReadMany<T>(string commandText, IDictionary<string, object> parameters, Func<IDataRecord, T> convertResultRow)
        {
            if (commandText == "SELECT * FROM ExpenseReportItems WHERE ExpenseReportId=@expenseReportId;")
            {
                return new List<T>
                {
                    convertResultRow(new ExpenseReportItemDataRecord("Invalid")),
                };
            }
            throw new InvalidOperationException();
        }

        public override bool TryReadManyInto<T>(T target, string commandText, IDictionary<string, object> parameters, Action<T, IDataRecord> updateTargetFromRow)
        {
            if (commandText == "SELECT * FROM ExpenseReports r LEFT JOIN ExpenseReportItems i ON r.Id = i.ExpenseReportId WHERE r.Id=@id;")
            {
                updateTargetFromRow(target, new ExpenseReportDataJoinRecord("Invalid"));
                return parameters["id"].Equals(1);
            }
            throw new InvalidOperationException();
        }
    }

    internal class MockSqlDataProvider : ISqlDataProvider
    {
        public virtual IEnumerable<T> ReadMany<T>(string commandText, IDictionary<string, object> parameters, Func<IDataRecord, T> convertResultRow)
        {
            if (commandText == "SELECT * FROM ExpenseReportItems WHERE ExpenseReportId=@expenseReportId;")
            {
                return new List<T>
                {
                    convertResultRow(new ExpenseReportItemDataRecord("O")),
                    convertResultRow(new ExpenseReportItemDataRecord("M")),
                    convertResultRow(new ExpenseReportItemDataRecord("L")),
                    convertResultRow(new ExpenseReportItemDataRecord("L*")),
                    convertResultRow(new ExpenseReportItemDataRecord("TL")),
                    convertResultRow(new ExpenseReportItemDataRecordWithNulls("TA")),
                    convertResultRow(new ExpenseReportItemDataRecordWithNulls("Ot")),
                };
            }
            throw new InvalidOperationException();
        }

        public virtual IEnumerable<T> ReadMany<T>(string commandText, Func<IDataRecord, T> convertResultRow)
        {
            if (commandText == "SELECT * FROM ExpenseReports ORDER BY ModifiedOn DESC;")
            {
                return new List<T>
                {
                    convertResultRow(new ExpenseReportDataRecord()),
                    convertResultRow(new ExpenseReportDataRecordWithNulls()),
                };
            }
            throw new InvalidOperationException();
        }

        public virtual bool TryReadManyInto<T>(T target, string commandText, IDictionary<string, object> parameters, Action<T, IDataRecord> updateTargetFromRow)
        {
            if (commandText == "SELECT * FROM ExpenseReports r LEFT JOIN ExpenseReportItems i ON r.Id = i.ExpenseReportId WHERE r.Id=@id;")
            {
                updateTargetFromRow(target, new ExpenseReportDataJoinRecord("M"));
                return parameters["id"].Equals(1);
            }
            throw new InvalidOperationException();
        }

        public virtual T ReadOne<T>(string commandText, IDictionary<string, object> parameters, Func<IDataRecord, T> convertResultRow, T defaultValue = default)
        {
            if (commandText == "SELECT TOP 1 ItemNumber FROM ExpenseReportItems WHERE ExpenseReportId = @id ORDER BY 1 DESC")
            {
                return convertResultRow(new IntegerDataRecord());
            }
            throw new InvalidOperationException();
        }

        public void Execute(string commandText, IDictionary<string, object> parameters = null)
        {
        }

        public int ExecuteWithResult(string commandText, IDictionary<string, object> parameters = null)
        {
            return 1;
        }

        public void CommitChanges()
        {
        }

        public void Dispose()
        {
        }
    }
}
