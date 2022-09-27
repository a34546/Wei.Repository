using Microsoft.EntityFrameworkCore;
using Wei.Repository;

namespace Wei.Repository.Test
{
    public class TestDbContext : BaseDbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        {
        }

        public DbSet<Test> Test { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
    }
}
