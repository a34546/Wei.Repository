using WebApiDemo.Data;

namespace WebApiDemo.Services.Impl
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetAllAsync(CancellationToken cancellationToken);
    }
}
