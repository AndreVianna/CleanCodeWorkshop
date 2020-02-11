using System.Collections.Generic;
using XPenC.DataAccess.Contracts.Schema;

namespace XPenC.DataAccess.Contracts
{
    public interface IExpenseReportSet
    {
        ExpenseReportEntity CreateRecordWithDefaults();

        IEnumerable<ExpenseReportEntity> GetAll();
        ExpenseReportEntity Find(int id);

        void Add(ExpenseReportEntity source);
        void Update(ExpenseReportEntity source);
        void Delete(int id);
    }
}