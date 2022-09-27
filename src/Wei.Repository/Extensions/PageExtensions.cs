using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wei.Repository
{
    public static class PageExtensions
    {

        public static async Task<Page<T>> ToPageAsync<T>(
            this IQueryable<T> query,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            if (pageIndex < 1) throw new ArgumentOutOfRangeException(nameof(pageIndex), "页码从1开始");
            int startIndex = (pageIndex - 1) * pageSize;
            int count = await query.CountAsync(cancellationToken).ConfigureAwait(false);
            var items = await query.Skip(startIndex)
                                   .Take(pageSize)
                                   .ToListAsync(cancellationToken)
                                   .ConfigureAwait(false);
            return new Page<T>(items, pageIndex, pageSize, count);
        }
        public static Page<T> ToPage<T>(
            this IQueryable<T> query,
            int pageIndex,
            int pageSize)
        {
            if (pageIndex < 1) throw new ArgumentOutOfRangeException(nameof(pageIndex), "页码从1开始");
            int startIndex = (pageIndex - 1) * pageSize;
            int count = query.Count();
            var items = query.Skip(startIndex)
                             .Take(pageSize)
                             .ToList();
            return new Page<T>(items, pageIndex, pageSize, count);
        }
    }
}
