using Microsoft.EntityFrameworkCore;
using WebApiDemo.Data;
using Wei.Repository;

namespace WebApiDemo
{
    public class UserReadonlyDbContext : BaseDbContext
    {
        public UserReadonlyDbContext(DbContextOptions<UserReadonlyDbContext> options) : base(options)
        {
        }

        public DbSet<User> User { get; set; }
    }
}
