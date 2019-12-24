using System;
using System.Collections.Generic;
using System.Text;

namespace Wei.Repository
{
    public interface IPage<out T>
    {
        /// <summary>
        /// 当前页码
        /// </summary>
        int PageIndex { get; }

        /// <summary>
        /// 每页记录数
        /// </summary>
        int PageSize { get; }

        /// <summary>
        /// 总记录数
        /// </summary>
        int Total { get; }

        /// <summary>
        /// 总页数
        /// </summary>
        int PageTotal { get; }

        /// <summary>
        /// 分页数据
        /// </summary>
        IEnumerable<T> Items { get; }
    }
}
