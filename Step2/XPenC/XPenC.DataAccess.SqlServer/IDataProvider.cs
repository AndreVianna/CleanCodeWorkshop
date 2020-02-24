namespace XPenC.DataAccess.SqlServer
{
    public interface IDataProvider<out TRowReader> : IQueryCommandsHandler<TRowReader>, INonQueryCommandsHandler
    {
    }
}