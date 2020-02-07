using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace XPenC.WebApp.DataAccess
{
    public class QueryCommand<T> : CommandWrapper
    {
        public QueryCommand(SqlCommand command) : base(command)
        {
        }

        public T ReadOne(Func<SqlDataReader, T> convertResult, T defaultValue = default)
        {
            using (var r = SqlCommand.ExecuteReader())
            {
                if (r.Read())
                    return convertResult(r);
                return defaultValue;
            }
        }

        public IEnumerable<T> ReadMany(Func<SqlDataReader, T> convertResult)
        {
            var result = new List<T>();
            using (var r = SqlCommand.ExecuteReader())
            {
                while (r.Read())
                {
                    result.Add(convertResult(r));
                }
            }
            return result;
        }

        public bool TryUpdate(T target, Action<T, SqlDataReader> updateTarget)
        {
            bool hasUpdates;
            using (var r = SqlCommand.ExecuteReader())
            {
                hasUpdates = r.HasRows;
                while (r.Read())
                {
                    updateTarget(target, r);
                }
            }
            return hasUpdates;
        }
    }
}