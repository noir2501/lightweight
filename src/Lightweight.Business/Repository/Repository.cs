using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using Lightweight.Model;

namespace Lightweight.Business.Repository
{
    public class Repository<TKey, T> : IKeyedRepository<TKey, T> where T : class, IEntity<TKey>
    {
        protected readonly ISession _session;
        public ISession Session { get { return _session; } }

        public Repository(ISession session)
        {
            _session = session;
        }

        public T FindById(TKey id)
        {
            return _session.Get<T>(id);
        }

        public TKey Add(T entity)
        {
            TKey id = (TKey)_session.Save(entity);

            return id;
        }

        public void Add(IEnumerable<T> items)
        {
            foreach (T item in items)
                _session.Save(item);
        }

        public TKey Insert(T entity)
        {
            BeginTransaction();
            TKey id = (TKey)_session.Save(entity);
            CommitTransaction();

            return id;
        }

        public void Insert(IEnumerable<T> items)
        {
            BeginTransaction();
            foreach (T item in items)
                _session.Save(item);
            CommitTransaction();
        }

        public void Update(T entity)
        {
            BeginTransaction();
            _session.Update(entity);
            CommitTransaction();
        }

        public void Update(IEnumerable<T> items)
        {
            BeginTransaction();
            foreach (T item in items)
                _session.Update(item);
            CommitTransaction();
        }

        public void Save(T entity)
        {
            BeginTransaction();
            _session.SaveOrUpdate(entity);
            CommitTransaction();
        }

        public void Delete(T entity)
        {
            BeginTransaction();
            _session.Delete(entity);
            CommitTransaction();
        }

        public void Delete(IEnumerable<T> entities)
        {
            BeginTransaction();
            foreach (T entity in entities)
                _session.Delete(entity);
            CommitTransaction();
        }

        public IQueryable<T> All()
        {
            return _session.Query<T>();
        }

        public T FindBy(System.Linq.Expressions.Expression<Func<T, bool>> expression)
        {
            return FilterBy(expression).SingleOrDefault();
        }

        public IQueryable<T> FilterBy(System.Linq.Expressions.Expression<Func<T, bool>> expression)
        {
            return All().Where(expression).AsQueryable();
        }

        public IQueryable<T> FilterByPaged(System.Linq.Expressions.Expression<Func<T, bool>> expression, int? pageIndex = null, int? pageSize = null)
        {
            var q = All().Where(expression).AsQueryable();

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                if (pageIndex < 1)
                    pageIndex = 1;

                if (pageSize.Value > 0 && pageSize < int.MaxValue)
                    q = q.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }

            return q;
        }

        public IKeyedRepository<TKey, T> BeginTransaction()
        {
            if (_session.Transaction != null && _session.Transaction.IsActive)
                return this; // no need to start a new transaction

            _session.BeginTransaction();

            return this;
        }

        public void CommitTransaction()
        {
            if (_session.Transaction.IsActive)
                try
                {
                    _session.Transaction.Commit();
                }
                catch
                {
                    RollbackTransaction();
                    throw;
                }
            else
                throw new InvalidOperationException("There is no active transaction to commit.");
        }

        public void RollbackTransaction()
        {
            if (_session.Transaction.IsActive)
                _session.Transaction.Rollback();
            else
                throw new InvalidOperationException("There is no active transaction to rollback.");
        }

    }
}