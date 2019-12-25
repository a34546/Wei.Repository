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

    public interface IRepository<TEntity, TPrimaryKey> where TEntity : class, IEntity<TPrimaryKey>
    {

        #region Get/Query
        /// <summary>
        /// 根据Id获取
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>Entity</returns>
        TEntity GetById(TPrimaryKey id);

        /// <summary>
        /// 根据Id获取,不跟踪实体变化
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>Entity</returns>
        TEntity GetByIdNoTracking(TPrimaryKey id);

        /// <summary>
        /// 获取全部(不包括逻辑删除的)
        /// </summary>
        List<TEntity> GetAllList();

        /// <summary>
        /// 获取全部(不包括逻辑删除的)
        /// </summary>
        List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 根据Id获取
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>Entity</returns>
        ValueTask<TEntity> GetByIdAsync(TPrimaryKey id);

        /// <summary>
        /// 根据Id获取,不跟踪实体变化
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>Entity</returns>
        ValueTask<TEntity> GetByIdNoTrackingAsync(TPrimaryKey id);

        /// <summary>
        /// 获取全部(不包括逻辑删除的)
        /// </summary>
        Task<List<TEntity>> GetAllListAsync();

        /// <summary>
        /// 获取全部(不包括逻辑删除的)
        /// </summary>
        Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 查询
        /// </summary>
        IQueryable<TEntity> Query { get; }

        /// <summary>
        /// 查询(不跟踪实体)
        /// </summary>
        IQueryable<TEntity> QueryNoTracking { get; }

        #endregion

        #region Insert
        /// <summary>
        /// 新增
        /// </summary>
        void Insert(TEntity entity);

        /// <summary>
        /// 批量新增
        /// </summary>
        void Insert(IEnumerable<TEntity> entities);

        /// <summary>
        /// 新增
        /// </summary>
        Task InsertAsync(TEntity entity);

        /// <summary>
        /// 批量新增
        /// </summary>
        Task InsertAsync(IEnumerable<TEntity> entities);

        #endregion

        #region Update
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">Entity</param>
        void Update(TEntity entity);

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="entities"></param>
        void Update(IEnumerable<TEntity> entities);

        /// <summary>
        /// 更新
        /// </summary>
        void Update(TEntity entity, params Expression<Func<TEntity, object>>[] properties);

        /// <summary>
        /// 更新
        /// </summary>
        Task UpdateAsync(TEntity entity);

        /// <summary>
        /// 批量更新
        /// </summary>
        Task UpdateAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// 更新
        /// </summary>
        Task UpdateAsync(TEntity entity, params Expression<Func<TEntity, object>>[] properties);
        #endregion

        #region Delete/HardDelete

        /// <summary>
        /// 删除
        /// </summary>
        void Delete(TPrimaryKey id);

        /// <summary>
        /// 删除
        /// </summary>
        void Delete(TEntity entity);

        /// <summary>
        /// 删除
        /// </summary>
        void Delete(IEnumerable<TEntity> entities);

        /// <summary>
        /// 删除
        /// </summary>
        void Delete(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 删除
        /// </summary>
        Task DeleteAsync(TPrimaryKey id);

        /// <summary>
        /// 删除
        /// </summary>
        Task DeleteAsync(TEntity entity);

        /// <summary>
        /// 删除
        /// </summary>
        Task DeleteAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// 删除
        /// </summary>
        Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 删除
        /// </summary>
        void HardDelete(TPrimaryKey id);

        /// <summary>
        /// 删除
        /// </summary>
        void HardDelete(TEntity entity);

        /// <summary>
        /// 删除
        /// </summary>
        void HardDelete(IEnumerable<TEntity> entities);

        /// <summary>
        /// 删除
        /// </summary>
        void HardDelete(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 删除
        /// </summary>
        Task HardDeleteAsync(TPrimaryKey id);

        /// <summary>
        /// 删除
        /// </summary>
        Task HardDeleteAsync(TEntity entity);

        /// <summary>
        /// 删除
        /// </summary>
        Task HardDeleteAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// 删除
        /// </summary>
        Task HardDeleteAsync(Expression<Func<TEntity, bool>> predicate);
        #endregion

    }
}
