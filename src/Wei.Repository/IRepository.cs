using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Wei.Repository
{
    public interface IRepository<TEntity> : IRepository<TEntity, int>
        where TEntity : class, IEntity
    {
    }

    public interface IRepository<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        #region Query

        IQueryable<TEntity> Query();

        IQueryable<TEntity> QueryNoTracking();

        IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate);

        TEntity Get(TPrimaryKey id);

        Task<TEntity> GetAsync(TPrimaryKey id);

        List<TEntity> GetAll();

        Task<List<TEntity>> GetAllAsync();

        List<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate);

        Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate);

        TEntity FirstOrDefault();

        Task<TEntity> FirstOrDefaultAsync();

        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

        #endregion

        #region Insert

        TEntity Insert(TEntity entity);

        Task<TEntity> InsertAsync(TEntity entity);

        void Insert(List<TEntity> entities);

        Task InsertAsync(List<TEntity> entities);

        #endregion Insert

        #region Update

        TEntity Update(TEntity entity);

        Task<TEntity> UpdateAsync(TEntity entity);

        #endregion Update

        #region Delete

        void Delete(TEntity entity);

        Task DeleteAsync(TEntity entity);

        void Delete(TPrimaryKey id);

        Task DeleteAsync(TPrimaryKey id);

        void Delete(Expression<Func<TEntity, bool>> predicate);

        Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);

        #endregion

        #region HardDelete

        void HardDelete(TEntity entity);

        Task HardDeleteAsync(TEntity entity);

        void HardDelete(TPrimaryKey id);

        Task HardDeleteAsync(TPrimaryKey id);

        void HardDelete(Expression<Func<TEntity, bool>> predicate);

        Task HardDeleteAsync(Expression<Func<TEntity, bool>> predicate);

        #endregion

        #region Aggregate

        bool Any(Expression<Func<TEntity, bool>> predicate);

        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);

        int Count();

        Task<int> CountAsync();

        int Count(Expression<Func<TEntity, bool>> predicate);

        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);

        long LongCount();

        Task<long> LongCountAsync();

        long LongCount(Expression<Func<TEntity, bool>> predicate);

        Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate);

        #endregion
    }
}
