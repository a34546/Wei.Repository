using WebApiDemo.Data;
using Wei.Repository;

namespace WebApiDemo
{
    public class BookRepository : Repository<Book>, IBookRepository
    {
        public BookRepository(DbContextFactory dbContextFactory) : base(dbContextFactory)
        {
        }

        public override Book Update(Book entity)
        {
            entity.Name += $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
            return base.Update(entity);
        }

    }

    public interface IBookRepository : IRepository<Book>
    {
    }
}
