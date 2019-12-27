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

        /// <summary>
        /// 查询
        /// </summary>
        IQueryable<TEntity> Query();
        IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 查询不跟踪实体变化
        /// </summary>
        IQueryable<TEntity> QueryNoTracking();
        IQueryable<TEntity> QueryNoTracking(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 根据主键获取
        /// </summary>
        TEntity Get(TPrimaryKey id);
        Task<TEntity> GetAsync(TPrimaryKey id);

        /// <summary>
        /// 获取所有,默认过滤IsDelete=1的
        /// </summary>
        List<TEntity> GetAll();
        Task<List<TEntity>> GetAllAsync();
        List<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate);
        Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 获取第一个
        /// </summary>
        TEntity FirstOrDefault();
        Task<TEntity> FirstOrDefaultAsync();
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

        #endregion

        #region Insert

        /// <summary>
        /// 新增
        /// </summary>
        TEntity Insert(TEntity entity);
        Task<TEntity> InsertAsync(TEntity entity);

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="entities"></param>
        void Insert(List<TEntity> entities);
        Task InsertAsync(List<TEntity> entities);

        #endregion Insert

        #region Update

        /// <summary>
        /// 更新
        /// </summary>
        TEntity Update(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity);

        #endregion Update

        #region Delete

        /// <summary>
        /// 逻辑删除，标记IsDelete = 1
        /// </summary>
        /// <param name="entity"></param>
        void Delete(TEntity entity);
        Task DeleteAsync(TEntity entity);
        void Delete(TPrimaryKey id);
        Task DeleteAsync(TPrimaryKey id);
        void Delete(Expression<Func<TEntity, bool>> predicate);
        Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);

        #endregion

        #region HardDelete

        /// <summary>
        /// 物理删除，从数据库中移除
        /// </summary>
        /// <param name="entity"></param>
        void HardDelete(TEntity entity);
        Task HardDeleteAsync(TEntity entity);
        void HardDelete(TPrimaryKey id);
        Task HardDeleteAsync(TPrimaryKey id);
        void HardDelete(Expression<Func<TEntity, bool>> predicate);
        Task HardDeleteAsync(Expression<Func<TEntity, bool>> predicate);

        #endregion

        #region Aggregate

        /// <summary>
        /// 聚合操作
        /// </summary>
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
