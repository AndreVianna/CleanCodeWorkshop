using System;
using System.Data;

namespace XPenC.DataAccess.SqlServer
{
    public interface ISqlDataProvider : IDataProvider<IDataRecord>, IDisposable
    {

    }
}