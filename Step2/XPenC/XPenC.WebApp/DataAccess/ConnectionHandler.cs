using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace XPenC.WebApp.DataAccess
{
    public class ConnectionHandler : IDisposable
    {
        private readonly SqlTransaction _transaction;
        private readonly SqlConnection _connection;

        public ConnectionHandler(IConfiguration configuration)
        {
            var cs = configuration["ConnectionStrings:DataContext"];
            _connection = new SqlConnection(cs);
            _connection.Open();
            _transaction = _connection.BeginTransaction();
        }

        private bool _isDisposed;

        protected virtual void Dispose(bool isDisposing)
        {
            if (_isDisposed) return;
            if (isDisposing)
            {
                _transaction?.Dispose();
                _connection?.Dispose();
            }
            _isDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public QueryCommand<T> CreateQueryCommand<T>(string commandText, IDictionary<string, object> parameters = null)
        {
            return (QueryCommand<T>)CreateCommand(commandText, parameters, command => new QueryCommand<T>(command));
        }

        public NonQueryCommand CreateNonQueryCommand(string commandText, IDictionary<string, object> parameters = null)
        {
            return (NonQueryCommand)CreateCommand(commandText, parameters, command => new NonQueryCommand(command));
        }

        private CommandWrapper CreateCommand(string commandText, IDictionary<string, object> parameters, Func<SqlCommand, CommandWrapper> create)
        {
            using (var command = _transaction.Connection.CreateCommand())
            {
                command.CommandText = commandText;
                command.Transaction = _transaction;
                if (parameters != null && parameters.Count > 0)
                {
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                    }
                }

                return create(command);
            }
        }

        public void CommitChanges()
        {
            _transaction.Commit();
        }
    }
}