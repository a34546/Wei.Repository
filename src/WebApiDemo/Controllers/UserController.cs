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
        public Task<User> FirstOrDefaultAsync(CancellationToken cancellationToken)
        {
            return _repository.FirstOrDefaultAsync(cancellationToken);
        }

        [HttpGet("GetAllAsync")]
        public Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken)
        {
            return _repository.GetAllAsync(cancellationToken);
        }

        [HttpPost]
        public async Task<User> InsertAsync(string name, CancellationToken cancellationToken)
        {
            var entity = await _userRepository.InsertAsync(new User { Name = name }, cancellationToken);
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