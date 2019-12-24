using System;
using System.Collections.Generic;
using System.Text;

namespace Wei.Repository
{
    /// <summary>
    /// 分页列表
    /// </summary>
    public class Page<T> : IPage<T>
    {
        public Page()
        {

        }
        public Page(IList<T> items, int pageIndex, int pageSize, int totalCount)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            Total = totalCount;
            PageTotal = (int)Math.Ceiling(totalCount / (double)pageSize);
            Items = items;
        }

        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 每页记录数
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int PageTotal { get; set; }

        /// <summary>
        /// 分页数据
        /// </summary>
        public IEnumerable<T> Items { get; set; }
    }
}
