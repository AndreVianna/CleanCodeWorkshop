using System;
using System.Data;

namespace XPenC.DataAccess.SqlServer.Tests.TestDoubles
{
    internal class DummyDataRecord : IDataRecord
    {
        public virtual object this[int i] => throw new NotImplementedException();
        public virtual object this[string name] => throw new NotImplementedException();
        public virtual int FieldCount => throw new NotImplementedException();
        public virtual bool GetBoolean(int i) => throw new NotImplementedException();
        public virtual byte GetByte(int i) => throw new NotImplementedException();
        public virtual long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length) => throw new NotImplementedException();
        public virtual char GetChar(int i) => throw new NotImplementedException();
        public virtual long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length) => throw new NotImplementedException();
        public virtual IDataReader GetData(int i) => throw new NotImplementedException();
        public virtual string GetDataTypeName(int i) => throw new NotImplementedException();
        public virtual DateTime GetDateTime(int i) => throw new NotImplementedException();
        public virtual decimal GetDecimal(int i) => throw new NotImplementedException();
        public virtual double GetDouble(int i) => throw new NotImplementedException();
        public virtual Type GetFieldType(int i) => throw new NotImplementedException();
        public virtual float GetFloat(int i) => throw new NotImplementedException();
        public virtual Guid GetGuid(int i) => throw new NotImplementedException();
        public virtual short GetInt16(int i) => throw new NotImplementedException();
        public virtual int GetInt32(int i) => throw new NotImplementedException();
        public virtual long GetInt64(int i) => throw new NotImplementedException();
        public virtual string GetName(int i) => throw new NotImplementedException();
        public virtual int GetOrdinal(string name) => throw new NotImplementedException();
        public virtual string GetString(int i) => throw new NotImplementedException();
        public virtual object GetValue(int i) => throw new NotImplementedException();
        public virtual int GetValues(object[] values) => throw new NotImplementedException();
        public virtual bool IsDBNull(int i) => throw new NotImplementedException();
    }
}