using System;
using Domain.Base;
using System.Linq;
using MongoDB.Entities;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;
using Application.Common.Extensions;
using Application.Common.Interfaces.Repositories;
using Domain.Enums.System;

namespace Persistence.Base
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : EntityBase
    {
        public void Delete(string id)
        {
            DB.GetInstance(typeof(TEntity).GetAttributeValue((DatabaseAttribute dbAttr) => dbAttr.Name)).Delete<TEntity>(id);
        }

        public void Delete(TEntity entity)
        {
            entity.Delete();
        }

        public async Task DeleteAsync(string id)
        {
            await DB.GetInstance(typeof(TEntity).GetAttributeValue((DatabaseAttribute dbAttr) => dbAttr.Name)).DeleteAsync<TEntity>(id);
        }

        public async Task DeleteAsync(TEntity entity)
        {
            await entity.DeleteAsync();
        }

        public List<TResult> Find<TResult>(
            Expression<Func<TEntity, bool>> condition, 
            Expression<Func<TEntity, TResult>> projection = null,
            Expression<Func<TEntity, object>> sortBy = null,
            Sort sortOrder = Sort.None,
            int skip = 0,
            int take = 0)
        {
            return FindAsync(condition, projection, sortBy, sortOrder, skip, take).GetAwaiter().GetResult();
        }

        public TEntity Find(string id)
        {
            return FindAsync<TEntity>(x => x.ID == id).GetAwaiter().GetResult().SingleOrDefault();
        }

        public TResult Find<TResult>(string id, Expression<Func<TEntity, TResult>> projection)
        {
            return FindAsync(x => x.ID == id, projection).GetAwaiter().GetResult().SingleOrDefault();
        }

        public List<TEntity> Find(Expression<Func<TEntity, bool>> condition)
        {
            return FindAsync<TEntity>(condition, null).GetAwaiter().GetResult();
        }

        public async Task<List<TResult>> FindAsync<TResult>(
            Expression<Func<TEntity, bool>> condition, 
            Expression<Func<TEntity, TResult>> projection = null,
            Expression<Func<TEntity, object>> sortBy = null,
            Sort sortOrder = Sort.None,
            int skip = 0,
            int take = 0)
        {
            var cmd = condition != null 
                ? DB.GetInstance(typeof(TEntity).GetAttributeValue((DatabaseAttribute dbAttr) => dbAttr.Name)).Find<TEntity, TResult>().Match(condition) 
                : DB.GetInstance(typeof(TEntity).GetAttributeValue((DatabaseAttribute dbAttr) => dbAttr.Name)).Find<TEntity, TResult>();
            
            if (projection != null)
                cmd.Project(projection);

            if (sortOrder != Sort.None && sortBy != null)
                cmd = cmd.Sort(
                    sortBy,
                    sortOrder == Sort.Ascending
                    ? Order.Ascending
                    : Order.Descending);

            if (skip > 0)
                cmd.Skip(skip);

            if (take > 0)
                cmd.Limit(take);

            return await cmd.ExecuteAsync();
        }

        public async Task<TEntity> FindAsync(string id)
        {
            return (await FindAsync<TEntity>(x => x.ID == id, null)).SingleOrDefault();
        }

        public async Task<TResult> FindAsync<TResult>(string id, Expression<Func<TEntity, TResult>> projection)
        {
            return (await FindAsync(x => x.ID == id, projection)).SingleOrDefault();
        }

        public async Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> condition = null)
        {
            return await FindAsync<TEntity>(condition, null);
        }

        public string Save(TEntity entity)
        {
            entity.Save();
            return entity.ID;
        }

        public async Task<string> SaveAsync(TEntity entity)
        {
            await entity.SaveAsync();
            return entity.ID;
        }
    }
}
