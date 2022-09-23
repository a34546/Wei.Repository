using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Wei.Repository
{
    public class BaseDbContext : DbContext
    {

        public BaseDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
