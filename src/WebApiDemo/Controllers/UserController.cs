using Microsoft.AspNetCore.Mvc;
using WebApiDemo.Data;
using Wei.Repository;

namespace WebApiDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        // 泛型仓储
        // 多个DbContext时，如果不传入指定DbContext，默认使用第一个注入的DbContext
        private readonly IRepository<UserReadonlyDbContext, User> _repository;

        //自定义仓储
        private readonly IUserRepository _userRepository;

        // 工作单元,
        // 如果不传入指定DbContext，默认使用第一个注入的DbContext
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
            //注意：Update已由自定义的仓储重写,这里会直接调用重写的方法
            entity = _userRepository.Update(entity);
            _unitOfWork.SaveChanges();
            return entity;
        }
    }
}