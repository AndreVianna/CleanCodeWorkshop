using System.Collections.Generic;

namespace XPenC.DataAccess.SqlServer
{
    public interface INonQueryCommandsHandler
    {
        void Execute(string commandText, IDictionary<string, object> parameters = null);
        int ExecuteWithResult(string commandText, IDictionary<string, object> parameters = null);
        void CommitChanges();
    }
}