using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace XPenC.DataAccess.SqlServer
{
    [ExcludeFromCodeCoverage]
    public sealed class SqlDataProvider : ISqlDataProvider
    {
        private readonly IDbTransaction _transaction;
        private readonly IDbConnection _connection;

        public SqlDataProvider(IConfiguration configuration, string connectionStringName)
        {
            var cs = configuration[$"ConnectionStrings:{connectionStringName}"];
            _connection = new SqlConnection(cs);
            _connection.Open();
            _transaction = _connection.BeginTransaction();
        }

        private bool _isDisposed;
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _transaction?.Dispose();
            _connection?.Dispose();
            _isDisposed = true;
        }


        public T ReadOne<T>(string commandText, IDictionary<string, object> parameters, Func<IDataRecord, T> convertResultRow, T defaultValue = default)
        {
            using (var command = CreateCommand(commandText, parameters))
            {
                return ReadOne(command, convertResultRow, defaultValue);
            }
        }

        public IEnumerable<T> ReadMany<T>(string commandText, Func<IDataRecord, T> convertResultRow)
        {
            return ReadMany(commandText, null, convertResultRow);
        }

        public IEnumerable<T> ReadMany<T>(string commandText, IDictionary<string, object> parameters, Func<IDataRecord, T> convertResultRow)
        {
            using (var command = CreateCommand(commandText, parameters))
            {
                return ReadMany(command, convertResultRow);
            }
        }

        public bool TryReadManyInto<T>(T target, string commandText, IDictionary<string, object> parameters, Action<T, IDataRecord> updateTargetFromRow)
        {
            using (var command = CreateCommand(commandText, parameters))
            {
                return TryReadManyInto(command, target, updateTargetFromRow);
            }
        }

        public void Execute(string commandText, IDictionary<string, object> parameters = null)
        {
            using (var command = CreateCommand(commandText, parameters))
            {
                command.ExecuteNonQuery();
            }
        }

        public int ExecuteWithResult(string commandText, IDictionary<string, object> parameters = null)
        {
            using (var command = CreateCommand(commandText, parameters))
            {
                return ExecuteWithResult(command);
            }
        }

        public void CommitChanges()
        {
            _transaction.Commit();
        }

        [SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "Only used by pre-defined queries.")]
        private IDbCommand CreateCommand(string commandText, IDictionary<string, object> parameters)
        {
            var command = _transaction.Connection.CreateCommand();
            command.CommandText = commandText;
            command.Transaction = _transaction;
            if (parameters == null || parameters.Count <= 0)
            {
                return command;
            }

            foreach (var parameter in parameters)
            {
                
                command.Parameters.Add(new SqlParameter(parameter.Key, parameter.Value ?? DBNull.Value));
            }
            return command;
        }

        private static T ReadOne<T>(IDbCommand command, Func<IDataRecord, T> convertRow, T defaultValue = default)
        {
            using (var row = command.ExecuteReader())
            {
                if (row.Read())
                {
                    return convertRow(row);
                }

                return defaultValue;
            }
        }

        private static IEnumerable<T> ReadMany<T>(IDbCommand command, Func<IDataRecord, T> convertRow)
        {
            var result = new List<T>();
            using (var row = command.ExecuteReader())
            {
                while (row.Read())
                {
                    result.Add(convertRow(row));
                }
            }
            return result;
        }

        private static bool TryReadManyInto<T>(IDbCommand command, T target, Action<T, IDataRecord> updateTargetFromRow)
        {
            var hasUpdates = false;
            using (var row = command.ExecuteReader())
            {
                while (row.Read())
                {
                    updateTargetFromRow(target, row);
                    hasUpdates = true;
                }
            }
            return hasUpdates;
        }

        private static int ExecuteWithResult(IDbCommand command)
        {
            using (var r = command.ExecuteReader())
            {
                r.Read();
                return Convert.ToInt32(r.GetValue(0));
            }
        }
    }
}