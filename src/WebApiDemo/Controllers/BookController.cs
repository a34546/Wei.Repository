using Microsoft.AspNetCore.Mvc;
using WebApiDemo.Data;
using Wei.Repository;

namespace WebApiDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookController : ControllerBase
    {
        // 泛型仓储
        // 多个DbContext时，如果不传入指定DbContext，默认使用第一个注入的DbContext
        private readonly IRepository<Book> _repository;

        //自定义仓储
        private readonly IBookRepository _bookRepository;

        // 工作单元,
        // 如果不传入指定DbContext，默认使用第一个注入的DbContext
        private readonly IUnitOfWork _unitOfWork;


        public BookController(IRepository<Book> repository,
            IBookRepository bookRepository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _bookRepository = bookRepository;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("FirstOrDefaultAsync")]
        public Task<Book> FirstOrDefaultAsync(CancellationToken cancellationToken)
        {
            return _repository.FirstOrDefaultAsync(cancellationToken);
        }

        [HttpGet("GetAllAsync")]
        public Task<IEnumerable<Book>> GetAllAsync(CancellationToken cancellationToken)
        {
            return _repository.GetAllAsync(cancellationToken);
        }

        [HttpPost]
        public async Task<Book> InsertAsync(string name, CancellationToken cancellationToken)
        {
            var entity = await _bookRepository.InsertAsync(new Book { Name = name }, cancellationToken);
            _unitOfWork.SaveChanges();
            return entity;
        }

        [HttpPut("Update/{id}")]
        public Book Update(int id, string name)
        {
            var entity = _bookRepository.Get(id);
            entity.Name = name;
            //注意：Update已由自定义的仓储重写,这里会直接调用重写的方法
            entity = _bookRepository.Update(entity);
            _unitOfWork.SaveChanges();
            return entity;
        }
    }
}