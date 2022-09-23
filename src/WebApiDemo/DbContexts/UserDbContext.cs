using Microsoft.EntityFrameworkCore;
using WebApiDemo.Data;
using Wei.Repository;

namespace WebApiDemo
{
    public class UserDbContext : BaseDbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
        }

        public DbSet<User> User { get; set; }
    }
}
