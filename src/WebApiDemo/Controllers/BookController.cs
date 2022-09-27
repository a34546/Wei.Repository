using Microsoft.AspNetCore.Mvc;
using WebApiDemo.Data;
using Wei.Repository;

namespace WebApiDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookController : ControllerBase
    {
        // ���Ͳִ�
        // ���DbContextʱ�����������ָ��DbContext��Ĭ��ʹ�õ�һ��ע���DbContext
        private readonly IRepository<Book> _repository;

        //�Զ���ִ�
        private readonly IBookRepository _bookRepository;

        // ������Ԫ,
        // ���������ָ��DbContext��Ĭ��ʹ�õ�һ��ע���DbContext
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
            //ע�⣺Update�����Զ���Ĳִ���д,�����ֱ�ӵ�����д�ķ���
            entity = _bookRepository.Update(entity);
            _unitOfWork.SaveChanges();
            return entity;
        }
    }
}