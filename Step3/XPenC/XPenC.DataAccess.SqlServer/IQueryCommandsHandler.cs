using System;
using System.Collections.Generic;

namespace XPenC.DataAccess.SqlServer
{
    public interface IQueryCommandsHandler<out TRowReader>
    {
        T ReadOne<T>(string commandText, IDictionary<string, object> parameters, Func<TRowReader, T> convertResultRow, T defaultValue = default);
        IEnumerable<T> ReadMany<T>(string commandText, Func<TRowReader, T> convertResultRow);
        IEnumerable<T> ReadMany<T>(string commandText, IDictionary<string, object> parameters, Func<TRowReader, T> convertResultRow);
        bool TryReadManyInto<T>(T target, string commandText, IDictionary<string, object> parameters, Action<T, TRowReader> updateTargetFromRow);
    }
}