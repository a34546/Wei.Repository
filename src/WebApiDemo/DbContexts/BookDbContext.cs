using Microsoft.EntityFrameworkCore;
using WebApiDemo.Data;
using Wei.Repository;

namespace WebApiDemo
{
    public class BookDbContext : BaseDbContext
    {
        public BookDbContext(DbContextOptions<BookDbContext> options) : base(options)
        {
        }

        public DbSet<Book> Book { get; set; }
    }
}
