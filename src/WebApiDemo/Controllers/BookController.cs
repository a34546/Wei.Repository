using Microsoft.AspNetCore.Mvc;
using WebApiDemo.Data;
using WebApiDemo.Services.Impl;
using Wei.Repository;

namespace WebApiDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        //自定义仓储
        private readonly IBookRepository _bookRepository;

        // 工作单元,
        // 如果不传入指定DbContext，默认使用第一个注入的DbContext
        private readonly IUnitOfWork _unitOfWork;


        public BookController(IBookService bookService,
            IBookRepository bookRepository,
            IUnitOfWork unitOfWork)
        {
            _bookService = bookService;
            _bookRepository = bookRepository;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("FirstOrDefaultAsync")]
        public Task<Book> FirstOrDefaultAsync()
        {
            return _bookRepository.FirstOrDefaultAsync(HttpContext.RequestAborted);
        }

        [HttpGet("GetAllAsync")]
        public Task<IEnumerable<Book>> GetAllAsync()
        {
            return _bookService.GetAllAsync(HttpContext.RequestAborted);
        }

        [HttpPost]
        public async Task<Book> InsertAsync(string name)
        {
            var entity = await _bookRepository.InsertAsync(new Book { Name = name }, HttpContext.RequestAborted);
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