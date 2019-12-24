using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Wei.Repository
{
    public interface IUnitOfWork : IDisposable
    {

        int SaveChanges();

        Task<int> SaveChangesAsync();

        #region command sql

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

        /// <summary>
        /// 分页查询 
        /// 用法：await _unitOfWork.QueryPagedAsync(1,10,"select * from post where isDelete = @isDelete order by id desc", new { isDelete = 1 });
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="pageIndex">当前页码,从1开始</param>
        /// <param name="pageSize">每页记录数,最大5000</param>
        /// <param name="sql">sql语句</param>
        /// <param name="sqlArgs">sql参数</param>
        /// <returns></returns>
        Task<Page<TEntity>> QueryPagedAsync<TEntity>(int pageIndex, int pageSize, string sql, object sqlArgs = null)
            where TEntity : class;
        #endregion

        /// <summary>
        /// 开启事务
        /// </summary>
        /// <returns></returns>
        IDbContextTransaction BeginTransaction();

        /// <summary>
        /// 获取DbConnection
        /// </summary>
        /// <returns></returns>
        IDbConnection GetConnection();
    }
}
