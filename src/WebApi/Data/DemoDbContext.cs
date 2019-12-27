using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Wei.Repository;

namespace WebApi.Data
{
    public class DemoDbContext : BaseDbContext
    {
        public DemoDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
