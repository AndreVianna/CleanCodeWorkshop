using System.Collections.Generic;
using System.Linq;
using XPenC.BusinessLogic.Contracts.Models;
using XPenC.DataAccess.Contracts;
using XPenC.DataAccess.Contracts.Sets;

namespace XPenC.BusinessLogic.Tests.TestDoubles
{
    internal class InMemoryExpenseReportSet : IExpenseReportSet
    {
        private readonly IDataContext _dataContext;
        private readonly List<ExpenseReport> _data = new List<ExpenseReport>();

        public InMemoryExpenseReportSet(IDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public void Add(ExpenseReport source)
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
            var report = _data.Find(i => i.Id == id);
            if (report == null)
            {
                return;
            }

            _data.Remove(report);
            var reportItems = _dataContext.ExpenseReportItems.GetAllFor(report.Id).ToList();
            foreach (var item in reportItems)
            {
                _dataContext.ExpenseReportItems.DeleteFrom(report.Id, item.ItemNumber);
            }
        }

        public ExpenseReport Find(int id)
        {
            var record = _data.FirstOrDefault(i => i.Id == id);
            if (record == null)
            {
                return null;
            }

            GetItems(record);
            return record;
        }

        private void GetItems(ExpenseReport record)
        {
            record.Items = _dataContext.ExpenseReportItems.GetAllFor(record.Id).ToList();
        }

        public IEnumerable<ExpenseReport> GetAll()
        {
            var list = _data;
            foreach (var item in list)
            {
                GetItems(item);
            }
            return list;
        }

        public void Update(ExpenseReport source)
        {
            Delete(source.Id);
            Add(source);
        }
    }
}