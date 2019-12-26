using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wei.Repository.Impl.DapperAdapter;

namespace Wei.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _dbContext;
        private bool _disposed = false;

        public UnitOfWork(DbContext context)
        {
            _dbContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }

        public Task<int> SaveChangesAsync()
        {
            return _dbContext.SaveChangesAsync();
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

        public async Task<Page<TEntity>> QueryPagedAsync<TEntity>(int pageIndex, int pageSize, string sql, object sqlArgs = null) where TEntity : class
        {
            if (pageSize < 1 || pageSize > 5000) throw new ArgumentOutOfRangeException(nameof(pageSize));
            if (pageIndex < 1) throw new ArgumentOutOfRangeException(nameof(pageIndex));
            var partedSql = PagingUtil.SplitSql(sql);
            ISqlAdapter sqlAdapter = null;
            if (_dbContext.Database.IsMySql()) sqlAdapter = new MysqlAdapter();
            if (_dbContext.Database.IsSqlServer()) sqlAdapter = new SqlServerAdapter();
            if (sqlAdapter == null) throw new Exception("Unsupported database type");
            sql = sqlAdapter.PagingBuild(ref partedSql, sqlArgs, (pageIndex - 1) * pageSize, pageSize);
            var sqlCount = PagingUtil.GetCountSql(partedSql);
            var conn = GetConnection();
            var totalCount = await conn.ExecuteScalarAsync<int>(sqlCount, sqlArgs);
            var items = await conn.QueryAsync<TEntity>(sql, sqlArgs);
            var pagedList = new Page<TEntity>(items.ToList(), pageIndex - 1, pageSize, totalCount);
            return pagedList;
        }

        public IDbContextTransaction BeginTransaction()
        {
            return _dbContext.Database.BeginTransaction();
        }

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
                    _dbContext.Dispose();
                }
            }
            _disposed = true;
        }

        public IDbConnection GetConnection()
        {
            return _dbContext.Database.GetDbConnection();
        }
    }
}
