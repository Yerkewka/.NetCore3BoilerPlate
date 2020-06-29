using System;
using Domain.Base;
using System.Linq;
using MongoDB.Entities;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;
using Application.Common.Extensions;
using Application.Common.Interfaces.Repositories;

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

        public List<TResult> Find<TResult>(Expression<Func<TEntity, bool>> condition, Expression<Func<TEntity, TResult>> projection = null)
        {
            return FindAsync(condition, projection).GetAwaiter().GetResult();
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
            return FindAsync(condition).GetAwaiter().GetResult();
        }

        public async Task<List<TResult>> FindAsync<TResult>(Expression<Func<TEntity, bool>> condition, Expression<Func<TEntity, TResult>> projection = null)
        {
            var cmd = condition != null 
                ? DB.GetInstance(typeof(TEntity).GetAttributeValue((DatabaseAttribute dbAttr) => dbAttr.Name)).Find<TEntity, TResult>().Match(condition) 
                : DB.GetInstance(typeof(TEntity).GetAttributeValue((DatabaseAttribute dbAttr) => dbAttr.Name)).Find<TEntity, TResult>();
            
            if (projection != null)
                cmd.Project(projection);

            return await cmd.ExecuteAsync();
        }

        public async Task<TEntity> FindAsync(string id)
        {
            return (await FindAsync(x => x.ID == id)).SingleOrDefault();
        }

        public async Task<TResult> FindAsync<TResult>(string id, Expression<Func<TEntity, TResult>> projection)
        {
            return (await FindAsync(x => x.ID == id, projection)).SingleOrDefault();
        }

        public async Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> condition = null)
        {
            return await FindAsync(condition);
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
