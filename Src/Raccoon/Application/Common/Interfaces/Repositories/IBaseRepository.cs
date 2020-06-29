using System;
using Domain.Base;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Application.Common.Interfaces.Repositories
{
    public interface IBaseRepository<TEntity> where TEntity : EntityBase
    {
        Task<List<TResult>> FindAsync<TResult>(Expression<Func<TEntity, bool>> condition, Expression<Func<TEntity, TResult>> projection = null);

        List<TResult> Find<TResult>(Expression<Func<TEntity, bool>> condition, Expression<Func<TEntity, TResult>> projection = null);

        TEntity Find(string id);

        Task<TEntity> FindAsync(string id);

        TResult Find<TResult>(string id, Expression<Func<TEntity, TResult>> projection);

        Task<TResult> FindAsync<TResult>(string id, Expression<Func<TEntity, TResult>> projection);

        List<TEntity> Find(Expression<Func<TEntity, bool>> condition);

        Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> condition = null);

        string Save(TEntity entity);

        Task<string> SaveAsync(TEntity entity);

        void Delete(string id);

        Task DeleteAsync(string id);

        void Delete(TEntity entity);

        Task DeleteAsync(TEntity entity);
    }
}
