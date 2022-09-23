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
        IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 查询不跟踪实体变化
        /// </summary>
        IQueryable<TEntity> QueryNoTracking();
        IQueryable<TEntity> QueryNoTracking(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 根据主键获取
        /// </summary>
        TEntity Get(object id);
        ValueTask<TEntity> GetAsync(object id, CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取所有,默认过滤IsDelete=1的
        /// </summary>
        IEnumerable<TEntity> GetAll();
        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
        IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate);
        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取第一个
        /// </summary>
        TEntity FirstOrDefault();
        Task<TEntity> FirstOrDefaultAsync(CancellationToken cancellationToken = default);
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        #endregion

        #region Insert

        /// <summary>
        /// 新增
        /// </summary>
        TEntity Insert(TEntity entity);
        Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="entities"></param>
        void Insert(IEnumerable<TEntity> entities);
        Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        #endregion Insert

        #region Update

        /// <summary>
        /// 更新
        /// </summary>
        TEntity Update(TEntity entity);
        void Update(IEnumerable<TEntity> entities);

        #endregion Update

        #region Delete

        /// <summary>
        /// 物理删除，从数据库中移除
        /// </summary>
        /// <param name="entity"></param>
        void Delete(TEntity entity);
        void Delete(object id);
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
