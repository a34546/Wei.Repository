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

        //�Զ���ִ�
        private readonly IBookRepository _bookRepository;

        // ������Ԫ,
        // ���������ָ��DbContext��Ĭ��ʹ�õ�һ��ע���DbContext
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
            //ע�⣺Update�����Զ���Ĳִ���д,�����ֱ�ӵ�����д�ķ���
            entity = _bookRepository.Update(entity);
            _unitOfWork.SaveChanges();
            return entity;
        }
    }
}