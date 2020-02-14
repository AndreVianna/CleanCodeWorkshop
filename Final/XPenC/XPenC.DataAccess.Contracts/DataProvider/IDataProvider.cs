namespace XPenC.DataAccess.Contracts.DataProvider
{
    public interface IDataProvider<out TRowReader> : IQueryCommandsHandler<TRowReader>, INonQueryCommandsHandler
    {
    }
}