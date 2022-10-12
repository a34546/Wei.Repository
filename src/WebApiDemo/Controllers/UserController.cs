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
            //注意：Update已由自定义的仓储重写,这里会直接调用重写的方法
            entity = _userRepository.Update(entity);
            _unitOfWork.SaveChanges();
            return entity;
        }
    }
}