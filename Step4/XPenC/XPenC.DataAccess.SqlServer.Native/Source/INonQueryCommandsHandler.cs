using System.Collections.Generic;

namespace XPenC.DataAccess.SqlServer.Native
{
    public interface INonQueryCommandsHandler
    {
        void Execute(string commandText, IDictionary<string, object> parameters = null);
        int ExecuteWithResult(string commandText, IDictionary<string, object> parameters = null);
        void CommitChanges();
    }
}