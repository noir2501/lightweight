using NHibernate;
using System;

namespace Lightweight.Business.Providers.NHibernateSession
{
    public class UnitOfWork : IUnitOfWork
    {
         private readonly ITransaction _transaction;

        public ISession Session { get; private set; }

        public UnitOfWork(ISession session)
        {

            Session = session;
            _transaction = Session.BeginTransaction();
        }

        public void Commit()
        {
            if (!_transaction.IsActive)
                throw new InvalidOperationException("There is no active transaction to commit.");
            _transaction.Commit();
        }

        public void Rollback()
        {
            if (_transaction.IsActive)
                _transaction.Rollback();
        }

        public void Dispose()
        {
            Session.Close();
        }
    }
}
