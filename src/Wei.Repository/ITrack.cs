using System;
using System.Collections.Generic;
using System.Text;

namespace Wei.Repository
{
    public interface ITrack
    {

        /// <summary>
        /// 创建时间
        /// </summary>
        DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        bool IsDelete { get; set; }

        /// <summary>
        /// 删除时间
        /// </summary>
        DateTime? DeleteTime { get; set; }
    }
}
