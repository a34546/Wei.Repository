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

        private readonly IRepository<User> _userRepository;
        private readonly IUserRepository _repository;
        public UserController(IRepository<User> userRepository,
            IUserRepository repository)
        {
            _userRepository = userRepository;
            _repository = repository;
        }

        [HttpGet]
        public async Task<User> Get()
        {
            return await _userRepository.FirstOrDefaultAsync();
            //return await _repository.FirstOrDefaultAsync();
        }
    }
}
