using WebApiDemo.Data;
using Wei.Repository;

namespace WebApiDemo.Services.Impl
{
    // 标记后，会自动注入
    [ScopedService]
    public class BookService : IBookService
    {
        private readonly IRepository<Book> _repository;
        public BookService(IRepository<Book> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Book>> GetAllAsync(CancellationToken cancellationToken)
        {
            var books = await _repository.GetAllAsync(cancellationToken);
            return books.OrderByDescending(x => x.Id).ToList();
        }
    }
}
