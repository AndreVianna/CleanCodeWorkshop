using System.Collections.Generic;
using XPenC.BusinessLogic.Contracts;
using XPenC.BusinessLogic.Contracts.Models;

namespace XPenC.WebApp.Tests.TestDoubles
{
    internal class DummyExpenseReportOperations : IExpenseReportOperations
    {
        public virtual void Add(ExpenseReport newExpenseReport) => throw new System.NotImplementedException();
        public virtual void AddItem(ExpenseReport source, ExpenseReportItem newItem) => throw new System.NotImplementedException();
        public virtual ExpenseReport CreateWithDefaults() => throw new System.NotImplementedException();
        public virtual void Delete(int id) => throw new System.NotImplementedException();
        public virtual ExpenseReport Find(int id) => throw new System.NotImplementedException();
        public virtual IEnumerable<ExpenseReport> GetList() => throw new System.NotImplementedException();
        public virtual void RemoveItem(ExpenseReport source, int itemNumber) => throw new System.NotImplementedException();
        public virtual void Update(ExpenseReport source) => throw new System.NotImplementedException();
    }
}