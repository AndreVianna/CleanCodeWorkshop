using System.Collections.Generic;
using System.Linq;
using XPenC.DataAccess.Contracts.Schema;
using XPenC.DataAccess.Contracts.Sets;

namespace XPenC.BusinessLogic.Tests.TestDoubles
{
    public class InMemoryExpenseReportItemSet : IExpenseReportItemSet
    {
        private readonly List<ExpenseReportItemEntity> _data = new List<ExpenseReportItemEntity>();

        public void AddTo(int expenseReportId, ExpenseReportItemEntity source)
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
            var item = Find(expenseReportId, itemNumber);
            if (item == null)
            {
                return;
            }

            _data.Remove(item);
        }

        private ExpenseReportItemEntity Find(int expenseReportId, int itemNumber)
        {
            return _data.Find(i => i.ExpenseReportId == expenseReportId && i.ItemNumber == itemNumber);
        }

        public IEnumerable<ExpenseReportItemEntity> GetAllFor(int expenseReportId)
        {
            return _data.Where(i => i.ExpenseReportId == expenseReportId);
        }
    }
}