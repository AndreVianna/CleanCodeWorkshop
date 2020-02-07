using Microsoft.Data.SqlClient;

namespace XPenC.WebApp.DataAccess
{
    public abstract class CommandWrapper
    {

        protected CommandWrapper(SqlCommand sqlCommand)
        {
            SqlCommand = sqlCommand;
        }

        protected SqlCommand SqlCommand { get; }
    }
}