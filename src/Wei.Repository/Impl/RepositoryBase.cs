using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Wei.Repository
{
    public abstract class RepositoryBase<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        #region Query
        public abstract IQueryable<TEntity> Query();

        public abstract IQueryable<TEntity> QueryNoTracking();

        public virtual IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate)
        {
            return Query().Where(predicate);
        }

        public virtual List<TEntity> GetAll()
        {
            return Query().ToList();
        }

        public virtual Task<List<TEntity>> GetAllAsync()
        {
            return Query().ToListAsync();
        }

        public virtual List<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate)
        {
            return Query().Where(predicate).ToList();
        }

        public virtual Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Query().Where(predicate).ToListAsync();
        }

        public virtual TEntity Get(TPrimaryKey id)
        {
            return FirstOrDefault(CreateEqualityExpressionForId(id));
        }

        public virtual async Task<TEntity> GetAsync(TPrimaryKey id)
        {
            return await FirstOrDefaultAsync(CreateEqualityExpressionForId(id));
        }

        public virtual TEntity FirstOrDefault()
        {
            return Query().FirstOrDefault();
        }

        public virtual Task<TEntity> FirstOrDefaultAsync()
        {
            return Query().FirstOrDefaultAsync();
        }

        public virtual TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return Query().FirstOrDefault(predicate);
        }

        public virtual Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Query().FirstOrDefaultAsync(predicate);
        }
        #endregion

        #region Insert
        public abstract TEntity Insert(TEntity entity);

        public abstract Task<TEntity> InsertAsync(TEntity entity);

        public abstract void Insert(List<TEntity> entities);

        public abstract Task InsertAsync(List<TEntity> entities);
        #endregion

        #region 修改
        public abstract TEntity Update(TEntity entity);

        public virtual Task<TEntity> UpdateAsync(TEntity entity)
        {
            return Task.FromResult(Update(entity));
        }
        #endregion

        #region Delete
        public abstract void Delete(TEntity entity);

        public virtual Task DeleteAsync(TEntity entity)
        {
            Delete(entity);
            return Task.CompletedTask;
        }

        public abstract void Delete(TPrimaryKey id);

        public virtual Task DeleteAsync(TPrimaryKey id)
        {
            Delete(id);
            return Task.CompletedTask;
        }

        public abstract void Delete(Expression<Func<TEntity, bool>> predicate);

        public virtual Task DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            Delete(predicate);
            return Task.CompletedTask;
        }
        #endregion

        #region HardDelete
        public abstract void HardDelete(TEntity entity);

        public virtual Task HardDeleteAsync(TEntity entity)
        {
            HardDelete(entity);
            return Task.CompletedTask;
        }

        public abstract void HardDelete(TPrimaryKey id);

        public virtual Task HardDeleteAsync(TPrimaryKey id)
        {
            HardDelete(id);
            return Task.CompletedTask;
        }

        public abstract void HardDelete(Expression<Func<TEntity, bool>> predicate);

        public virtual Task HardDeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            HardDelete(predicate);
            return Task.CompletedTask;
        }
        #endregion

        #region Aggregate
        public bool Any(Expression<Func<TEntity, bool>> predicate)
        {
            return Query().Any(predicate);
        }

        public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Query().AnyAsync(predicate);
        }

        public virtual int Count()
        {
            return Query().Count();
        }

        public virtual Task<int> CountAsync()
        {
            return Query().CountAsync();
        }

        public virtual int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return Query().Where(predicate).Count();
        }

        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Query().CountAsync(predicate);
        }

        public virtual long LongCount()
        {
            return Query().LongCount();
        }

        public virtual Task<long> LongCountAsync()
        {
            return Query().LongCountAsync();
        }

        public virtual long LongCount(Expression<Func<TEntity, bool>> predicate)
        {
            return Query().Where(predicate).LongCount();
        }

        public virtual Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Query().LongCountAsync(predicate);
        }

        protected static Expression<Func<TEntity, bool>> CreateEqualityExpressionForId(TPrimaryKey id)
        {
            var lambdaParam = Expression.Parameter(typeof(TEntity));

            var lambdaBody = Expression.Equal(
                Expression.PropertyOrField(lambdaParam, "Id"),
                Expression.Constant(id, typeof(TPrimaryKey))
            );

            return Expression.Lambda<Func<TEntity, bool>>(lambdaBody, lambdaParam);
        }
        #endregion
    }
}
