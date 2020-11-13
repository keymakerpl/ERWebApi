using System;
using System.Transactions;

namespace ERService.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private TransactionScope _transactionScope;

        public void StartTransaction()
        {
            _transactionScope = new TransactionScope();
        }

        public void CommitTransaction()
        {
            _transactionScope.Complete();
        }

        public void Dispose()
        {
            _transactionScope.Dispose();
        }
    }
}
