using Microsoft.EntityFrameworkCore;
using WebApiDemo.Data;
using WebApiDemo.Entities;
using Wei.Repository;

namespace WebApiDemo
{
    public class UserBookDbContext : BaseDbContext
    {
        public UserBookDbContext(DbContextOptions<UserBookDbContext> options) : base(options)
        {
        }

        public DbSet<UserBook> UserBook { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 指定复合主键
            modelBuilder.Entity<UserBook>().HasKey(x => new { x.UserId, x.BookId });

        }
    }
}
