using System;
using System.Collections.Generic;
using System.Data;

namespace XPenC.DataAccess.SqlServer.Tests.TestDoubles
{
    internal class MockSqlDataProvider : ISqlDataProvider
    {
        public IEnumerable<T> ReadMany<T>(string commandText, IDictionary<string, object> parameters, Func<IDataRecord, T> convertResultRow)
        {
            return new List<T>
            {
                convertResultRow(new MockDataRecordWithNoNulls(6)),
                convertResultRow(new MockDataRecordWithNulls(6)),
            };
        }

        public IEnumerable<T> ReadMany<T>(string commandText, Func<IDataRecord, T> convertResultRow)
        {
            return new List<T>
            {
                convertResultRow(new MockDataRecordWithNoNulls(6)),
                convertResultRow(new MockDataRecordWithNulls(6)),
            };
        }

        public bool TryReadManyInto<T>(T target, string commandText, IDictionary<string, object> parameters, Action<T, IDataRecord> updateTargetFromRow)
        {
            updateTargetFromRow(target, new MockDataRecordWithNoNulls(12));
            return parameters["id"].Equals(1);
        }

        public T ReadOne<T>(string commandText, IDictionary<string, object> parameters, Func<IDataRecord, T> convertResultRow, T defaultValue = default)
        {
            return convertResultRow(new MockDataRecordWithNoNulls(6));
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
