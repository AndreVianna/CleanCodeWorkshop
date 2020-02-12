using System.Collections.Generic;
using XPenC.DataAccess.Contracts;
using XPenC.DataAccess.Contracts.Schema;

namespace XPenC.BusinessLogic.Tests
{
    public class InMemoryExpenseReportSet : IExpenseReportSet
    {
        private List<ExpenseReportEntity> _data = new List<ExpenseReportEntity>();

        public void Add(ExpenseReportEntity source)
        {
            source.Id = _data.Count + 1;
            _data.Add(source);
        }

        public ExpenseReportEntity CreateRecordWithDefaults()
        {
            return new ExpenseReportEntity();
        }

        public void Delete(int id)
        {
            var item = Find(id);
            if (item == null) return;
            _data.Remove(item);
        }

        public ExpenseReportEntity Find(int id)
        {
            return _data.Find(i => i.Id == id);
        }

        public IEnumerable<ExpenseReportEntity> GetAll()
        {
            return _data;
        }

        public void Update(ExpenseReportEntity source)
        {
            var item = Find(source.Id);
            if (item == null) return;
            _data.Remove(item);
            _data.Add(source);
        }
    }
}