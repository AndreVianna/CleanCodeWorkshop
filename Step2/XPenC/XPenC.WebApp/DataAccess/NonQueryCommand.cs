using Microsoft.Data.SqlClient;

namespace XPenC.WebApp.DataAccess
{
    public class NonQueryCommand : CommandWrapper
    {
        public NonQueryCommand(SqlCommand sqlCommand) : base(sqlCommand)
        {
        }

        public void Execute()
        {
            SqlCommand.ExecuteNonQuery();
        }

        public int ExecuteWithResult()
        {
            using (var r = SqlCommand.ExecuteReader())
            {
                r.Read();
                return r.GetInt32(0);
            }
        }
    }
}