using NHibernate;
using System;

namespace Lightweight.Business.Providers.NHibernateSession
{
    public interface IUnitOfWork : IDisposable
    {
        ISession Session { get; }
        void Commit();
        void Rollback();
    }
}
