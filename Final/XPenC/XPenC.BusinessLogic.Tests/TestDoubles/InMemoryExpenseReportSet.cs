using System.Collections.Generic;
using System.Linq;
using XPenC.DataAccess.Contracts;
using XPenC.DataAccess.Contracts.Schema;
using XPenC.DataAccess.Contracts.Sets;

namespace XPenC.BusinessLogic.Tests.TestDoubles
{
    internal class InMemoryExpenseReportSet : IExpenseReportSet
    {
        private readonly IDataContext _dataContext;
        private readonly List<ExpenseReportEntity> _data = new List<ExpenseReportEntity>();

        public InMemoryExpenseReportSet(IDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public void Add(ExpenseReportEntity source)
        {
            source.Id = _data.Count + 1;
            _data.Add(source);
            foreach (var item in source.Items)
            {
                _dataContext.ExpenseReportItems.AddTo(source.Id, item);
            }
        }

        public void Delete(int id)
        {
            var report = Find(id);
            if (report == null)
            {
                return;
            }

            _data.Remove(report);
            foreach (var item in report.Items)
            {
                _dataContext.ExpenseReportItems.DeleteFrom(report.Id, item.ItemNumber);
            }
        }

        public ExpenseReportEntity Find(int id)
        {
            var record = _data.Find(i => i.Id == id);
            if (record == null)
            {
                return null;
            }

            GetItems(record);
            return record;
        }

        private void GetItems(ExpenseReportEntity record)
        {
            record.Items = _dataContext.ExpenseReportItems.GetAllFor(record.Id).ToList();
        }

        public IEnumerable<ExpenseReportEntity> GetAll()
        {
            var list = _data;
            foreach (var item in list)
            {
                GetItems(item);
            }
            return list;
        }

        public void Update(ExpenseReportEntity source)
        {
            Delete(source.Id);
            Add(source);
        }
    }
}