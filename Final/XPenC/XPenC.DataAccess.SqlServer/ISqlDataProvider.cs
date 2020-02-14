using System;
using System.Data;
using XPenC.DataAccess.Contracts.DataProvider;

namespace XPenC.DataAccess.SqlServer
{
    public interface ISqlDataProvider : IDataProvider<IDataRecord>, IDisposable
    {

    }
}