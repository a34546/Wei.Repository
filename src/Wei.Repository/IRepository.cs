using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Wei.Repository
{

    public interface IRepository<TEntity> : IRepositoryBase<TEntity>
       where TEntity : class
    {

    }

    public interface IRepository<TDbContext, TEntity> : IRepositoryBase<TEntity>
        where TEntity : class
        where TDbContext : DbContext
    {

    }

    public interface IRepositoryBase<TEntity>
        where TEntity : class
    {
        #region Query

        /// <summary>
        /// 查询
        /// </summary>
        IQueryable<TEntity> Query();
        /// <summary>
        /// 查询
        /// </summary>
        IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 查询不跟踪实体变化
        /// </summary>
        IQueryable<TEntity> QueryNoTracking();
        /// <summary>
        /// 查询不跟踪实体变化
        /// </summary>
        IQueryable<TEntity> QueryNoTracking(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 根据主键获取
        /// </summary>
        TEntity Get(params object[] id);
        /// <summary>
        /// 根据主键获取(支持复合主键)
        /// </summary>
        ValueTask<TEntity> GetAsync(params object[] id);
        /// <summary>
        /// 根据主键(复合主键)
        /// </summary>
        ValueTask<TEntity> GetAsync(object[] ids, CancellationToken cancellationToken);

        /// <summary>
        /// 获取所有
        /// </summary>
        IEnumerable<TEntity> GetAll();
        /// <summary>
        /// 获取所有
        /// </summary>
        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// 根据表达式条件获取所有
        /// </summary>
        IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate);
        /// <summary>
        /// 根据表达式条件获取所有
        /// </summary>
        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取第一个或默认值
        /// </summary>
        TEntity FirstOrDefault();
        /// <summary>
        /// 获取第一个或默认值
        /// </summary>
        Task<TEntity> FirstOrDefaultAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// 根据表达式条件获取第一个或默认值
        /// </summary>
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);
        /// <summary>
        /// 获取第一个或默认值
        /// </summary>
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        #endregion

        #region Insert

        /// <summary>
        /// 新增
        /// </summary>
        TEntity Insert(TEntity entity);
        /// <summary>
        /// 新增
        /// </summary>
        Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// 批量新增
        /// </summary>
        void Insert(IEnumerable<TEntity> entities);
        /// <summary>
        /// 批量新增
        /// </summary>
        Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        #endregion Insert

        #region Update

        /// <summary>
        /// 更新
        /// </summary>
        TEntity Update(TEntity entity);
        /// <summary>
        /// 批量更新
        /// </summary>
        void Update(IEnumerable<TEntity> entities);

        /// <summary>
        /// 根据查询条件更新指定字段
        /// </summary>
        IEnumerable<TEntity> Update(Expression<Func<TEntity, bool>> predicate, Action<TEntity> updateAction);

        /// <summary>
        /// 根据查询条件更新指定字段
        /// </summary>
        Task<IEnumerable<TEntity>> UpdateAsync(Expression<Func<TEntity, bool>> predicate, Action<TEntity> updateAction, CancellationToken cancellationToken = default);

        #endregion Update

        #region Delete

        /// <summary>
        /// 根据传入的实体删除
        /// </summary>
        void Delete(TEntity entity);

        /// <summary>
        /// 根据主键(支持复合主键)删除
        /// </summary>
        void Delete(params object[] id);

        /// <summary>
        /// 根据表达式条件批量删除
        /// </summary>
        void Delete(Expression<Func<TEntity, bool>> predicate);

        #endregion

        #region Aggregate
        bool Any();
        Task<bool> AnyAsync(CancellationToken cancellationToken = default);
        bool Any(Expression<Func<TEntity, bool>> predicate);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        int Count();
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        int Count(Expression<Func<TEntity, bool>> predicate);
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        #endregion
    }
}
