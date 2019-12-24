using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wei.Repository
{
    public static class PagedListExtensions
    {
        /// <summary>
        /// PagedList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="pageIndex">1为起始页</param>
        /// <param name="pageSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<Page<T>> ToPagedListAsync<T>(
            this IQueryable<T> query,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            if (pageIndex < 1) throw new ArgumentOutOfRangeException(nameof(pageIndex));
            int realIndex = pageIndex - 1;
            int count = await query.CountAsync(cancellationToken).ConfigureAwait(false);
            var items = await query.Skip(realIndex * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync(cancellationToken)
                                   .ConfigureAwait(false);
            return new Page<T>(items, pageIndex, pageSize, count);
        }
        public static Page<T> ToPagedList<T>(
            this IQueryable<T> query,
            int pageIndex,
            int pageSize)
        {
            if (pageIndex < 1) throw new ArgumentOutOfRangeException(nameof(pageIndex));
            int realIndex = pageIndex - 1;
            int count = query.Count();
            var items = query.Skip(realIndex * pageSize)
                             .Take(pageSize)
                             .ToList();
            return new Page<T>(items, pageIndex, pageSize, count);
        }
    }
}
