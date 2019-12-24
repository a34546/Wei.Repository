using System;
using System.Collections.Generic;
using System.Text;
using Wei.Repository.Impl.DapperAdapter;

namespace Wei.Repository
{
    public interface ISqlAdapter
    {
        string PagingBuild(ref PartedSql partedSql, object sqlArgs, long skip, long take);
    }
}
