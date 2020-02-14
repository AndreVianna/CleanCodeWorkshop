using System.Collections.Generic;

namespace XPenC.DataAccess.Contracts.DataProvider
{
    public interface INonQueryCommandsHandler
    {
        void Execute(string commandText, IDictionary<string, object> parameters = null);
        int ExecuteWithResult(string commandText, IDictionary<string, object> parameters = null);
        void CommitChanges();
    }
}