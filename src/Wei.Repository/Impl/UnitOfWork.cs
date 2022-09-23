using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Wei.Repository
{
    public class UnitOfWork : UnitOfWorkBase, IUnitOfWork
    {
        public UnitOfWork(DbContextFactory dbContextFactory)
        {
            DbContext = dbContextFactory.GetFirstOrDefaultDbContext();
        }

        public override DbContext DbContext { get; protected set; }
    }

    public class UnitOfWork<TDbContext> : UnitOfWorkBase, IUnitOfWork<TDbContext> where TDbContext : DbContext
    {
        public UnitOfWork(DbContextFactory dbContextFactory)
        {
            DbContext = dbContextFactory.GetDbContext<TDbContext>();
        }

        public override DbContext DbContext { get; protected set; }

    }

    public abstract class UnitOfWorkBase : IUnitOfWork
    {
        private bool _disposed = false;

        public abstract DbContext DbContext { get; protected set; }

        public int SaveChanges()
        {
            return DbContext.SaveChanges();
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return DbContext.SaveChangesAsync(cancellationToken);
        }

        public Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string sql, object param = null, IDbContextTransaction trans = null) where TEntity : class
        {
            var conn = GetConnection();
            var result = conn.QueryAsync<TEntity>(sql, param, trans?.GetDbTransaction());
            return result;
        }

        public async Task<int> ExecuteAsync(string sql, object param, IDbContextTransaction trans = null)
        {
            var conn = GetConnection();
            return await conn.ExecuteAsync(sql, param, trans?.GetDbTransaction());
        }
        public IDbContextTransaction BeginTransaction() => DbContext.Database.BeginTransaction();
        public IDbContextTransaction BeginTransaction(IsolationLevel isolationLevel) => DbContext.Database.BeginTransaction(isolationLevel);
        public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default) => DbContext.Database.BeginTransactionAsync(cancellationToken);
        public Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken = default) => DbContext.Database.BeginTransactionAsync(isolationLevel, cancellationToken);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    DbContext.Dispose();
                }
            }
            _disposed = true;
        }

        public IDbConnection GetConnection() => DbContext.Database.GetDbConnection();
    }
}
