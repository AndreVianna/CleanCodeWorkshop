using System.Collections.Generic;
using System.Linq;
using XPenC.BusinessLogic.Contracts.Models;
using XPenC.DataAccess.Contracts.Sets;

namespace XPenC.BusinessLogic.Tests.TestDoubles
{
    internal class InMemoryExpenseReportItemSet : IExpenseReportItemSet
    {
        private readonly List<ExpenseReportItem> _data = new List<ExpenseReportItem>();

        public void AddTo(int expenseReportId, ExpenseReportItem source)
        {
            source.ExpenseReportId = expenseReportId;
            if (source.ItemNumber == 0)
            {
                source.ItemNumber = GetAllFor(source.ExpenseReportId).Select(i => i.ItemNumber).DefaultIfEmpty().Max() + 1;
            }
            _data.Add(source);
        }

        public void DeleteFrom(int expenseReportId, int itemNumber)
        {
            var item = _data.Find(i => i.ExpenseReportId == expenseReportId && i.ItemNumber == itemNumber);
            if (item == null)
            {
                return;
            }

            _data.Remove(item);
        }

        public IEnumerable<ExpenseReportItem> GetAllFor(int expenseReportId)
        {
            return _data.Where(i => i.ExpenseReportId == expenseReportId);
        }
    }
}