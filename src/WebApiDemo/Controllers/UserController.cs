using Microsoft.AspNetCore.Mvc;
using WebApiDemo.Data;
using Wei.Repository;

namespace WebApiDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        // ���Ͳִ�
        // ���DbContextʱ�����������ָ��DbContext��Ĭ��ʹ�õ�һ��ע���DbContext
        private readonly IRepository<UserReadonlyDbContext, User> _repository;

        //�Զ���ִ�
        private readonly IUserRepository _userRepository;

        // ������Ԫ,
        // ���������ָ��DbContext��Ĭ��ʹ�õ�һ��ע���DbContext
        private readonly IUnitOfWork<UserDbContext> _unitOfWork;


        public UserController(IRepository<UserReadonlyDbContext, User> repository,
            IUserRepository userRepository,
            IUnitOfWork<UserDbContext> unitOfWork)
        {
            _repository = repository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("FirstOrDefaultAsync")]
        public Task<User> FirstOrDefaultAsync()
        {
            return _repository.FirstOrDefaultAsync(HttpContext.RequestAborted);
        }

        [HttpGet("GetAllAsync")]
        public Task<IEnumerable<User>> GetAllAsync()
        {
            return _repository.GetAllAsync(HttpContext.RequestAborted);
        }

        [HttpPost]
        public async Task<User> InsertAsync(string name)
        {
            var entity = await _userRepository.InsertAsync(new User { Name = name }, HttpContext.RequestAborted);
            _unitOfWork.SaveChanges();
            return entity;
        }

        [HttpPut("Update/{id}")]
        public User Update(string id, string name)
        {
            var entity = _userRepository.Get(id);
            entity.Name = name;
            //ע�⣺Update�����Զ���Ĳִ���д,�����ֱ�ӵ�����д�ķ���
            entity = _userRepository.Update(entity);
            _unitOfWork.SaveChanges();
            return entity;
        }
    }
}