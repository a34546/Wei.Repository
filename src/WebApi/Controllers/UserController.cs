using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApi.Data;
using Wei.Repository;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {

        /// <summary>
        /// 泛型注入
        /// </summary>
        private readonly IRepository<User> _repository;

        /// <summary>
        /// 自定义UserRepository
        /// </summary>
        private readonly IUserRepository _userRepository;
        public UserController(IRepository<User> repository,
            IUserRepository userRepository)
        {
            _repository = repository;
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<User> Get()
        {
            //泛型注入不会调用重写的方法
            return await _repository.FirstOrDefaultAsync();

            //会调用重写的FirstOrDefaultAsync()
            //return await _userRepository.FirstOrDefaultAsync();
        }
    }
}
