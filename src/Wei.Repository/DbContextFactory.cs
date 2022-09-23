using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Wei.Repository
{
    public class DbContextFactory
    {
        private readonly IEnumerable<DbContext> _dbContexts;
        public DbContextFactory(IEnumerable<DbContext> dbContexts)
        {
            _dbContexts = dbContexts;
        }

        public DbContext GetFirstOrDefaultDbContext() => _dbContexts.FirstOrDefault();

        public DbContext GetDbContext<TDbContext>() where TDbContext : DbContext => _dbContexts.FirstOrDefault(x => x.GetType() == typeof(TDbContext));
    }
}
