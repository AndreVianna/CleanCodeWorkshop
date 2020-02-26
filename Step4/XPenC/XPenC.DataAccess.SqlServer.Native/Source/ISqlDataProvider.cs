using System;
using System.Data;

namespace XPenC.DataAccess.SqlServer.Native
{
    public interface ISqlDataProvider : IDataProvider<IDataRecord>, IDisposable
    {

    }
}