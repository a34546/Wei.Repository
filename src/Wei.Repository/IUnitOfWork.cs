using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wei.Repository
{
    public interface IUnitOfWork: IDisposable
    {

        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 查询
        /// 用法:await _unitOfWork.QueryAsync`Demo`("select id,title from post where id = @id", new { id = 1 });
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="trans"></param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string sql, object param = null, IDbContextTransaction trans = null) where TEntity : class;

        /// <summary>
        /// ExecuteAsync
        /// 用法:await _unitOfWork.ExecuteAsync("update post set title =@title where id =@id", new { title = "", id=1 });
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="trans"></param>
        /// <returns></returns>
        Task<int> ExecuteAsync(string sql, object param, IDbContextTransaction trans = null);

        IDbContextTransaction BeginTransaction();
        public IDbContextTransaction BeginTransaction(IsolationLevel isolationLevel);
        public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
        public Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken = default);
        IDbConnection GetConnection();
    }

    public interface IUnitOfWork<TDbContext> : IDisposable where TDbContext : DbContext
    {

        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 查询
        /// 用法:await _unitOfWork.QueryAsync`Demo`("select id,title from post where id = @id", new { id = 1 });
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="trans"></param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string sql, object param = null, IDbContextTransaction trans = null) where TEntity : class;

        /// <summary>
        /// ExecuteAsync
        /// 用法:await _unitOfWork.ExecuteAsync("update post set title =@title where id =@id", new { title = "", id=1 });
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="trans"></param>
        /// <returns></returns>
        Task<int> ExecuteAsync(string sql, object param, IDbContextTransaction trans = null);

        IDbContextTransaction BeginTransaction();
        public IDbContextTransaction BeginTransaction(IsolationLevel isolationLevel);
        public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
        public Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken = default);
        IDbConnection GetConnection();
    }
}
