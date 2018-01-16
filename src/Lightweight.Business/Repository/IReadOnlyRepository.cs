using System;
using System.Linq;
using System.Linq.Expressions;

namespace Lightweight.Business.Repository
{
    public interface IReadOnlyRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> All();
        IQueryable<TEntity> FilterBy(Expression<Func<TEntity, bool>>  expression);
        IQueryable<TEntity> FilterByPaged(Expression<Func<TEntity, bool>> expression, int? pageIndex = null, int? pageSize = null);

        TEntity FindBy(Expression<Func<TEntity, bool>> expression);    
    }
}
