using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wei.Repository
{
    public class Repository<TEntity>
       : RepositoryBase<TEntity>, IRepository<TEntity>
       where TEntity : class
    {
        internal override DbContext DbContext { get; set; }
        public Repository(DbContextFactory dbContextFactory)
        {
            DbContext = dbContextFactory.GetFirstOrDefaultDbContext();
        }

    }

    public class Repository<TDbContext, TEntity>
        : RepositoryBase<TEntity>, IRepository<TDbContext, TEntity>
        where TEntity : class
        where TDbContext : DbContext
    {
        internal override DbContext DbContext { get; set; }

        public Repository(DbContextFactory dbContextFactory)
        {
            DbContext = dbContextFactory.GetDbContext<TDbContext>();
        }
    }
}
