namespace XPenC.DataAccess.SqlServer.Native
{
    public interface IDataProvider<out TRowReader> : IQueryCommandsHandler<TRowReader>, INonQueryCommandsHandler
    {
    }
}