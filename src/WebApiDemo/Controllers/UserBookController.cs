using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using WebApiDemo.Data;
using WebApiDemo.Entities;
using Wei.Repository;

namespace WebApiDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserBookController : ControllerBase
    {
        // 泛型仓储
        // 多个DbContext时，如果不传入指定DbContext，默认使用第一个注入的DbContext
        private readonly IRepository<UserBookDbContext, UserBook> _repository;

        // 工作单元,
        // 如果不传入指定DbContext，默认使用第一个注入的DbContext
        private readonly IUnitOfWork<UserBookDbContext> _unitOfWork;


        public UserBookController(IRepository<UserBookDbContext, UserBook> repository,
            IUnitOfWork<UserBookDbContext> unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("FirstOrDefaultAsync")]
        public Task<UserBook> FirstOrDefaultAsync(CancellationToken cancellationToken)
        {
            return _repository.FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// 根据复合主键获取
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="bookId"></param>
        [HttpGet("GetByCompositeKey")]
        public async Task<UserBook> GetByCompositeKey([Required] string userId, [Required] int bookId, CancellationToken cancellationToken)
        {
            var entity = await _unitOfWork.DbContext.Set<UserBook>().FindAsync(userId, bookId);
            return entity;
        }

        [HttpGet("GetAllAsync")]
        public Task<IEnumerable<UserBook>> GetAllAsync(CancellationToken cancellationToken)
        {
            return _repository.GetAllAsync(cancellationToken);
        }

        [HttpGet("InsertAsync")]
        public async Task<UserBook> InsertAsync([Required] string userId, [Required] int bookId, [Required] string name, CancellationToken cancellationToken)
        {
            var entity = await _repository.InsertAsync(new UserBook { BookId = bookId, UserId = userId, Name = name }, cancellationToken);
            _unitOfWork.SaveChanges();
            return entity;
        }

        [HttpGet("Update/{userId}/{bookId}")]
        public UserBook Update(string userId, int bookId, string name)
        {
            var entity = _unitOfWork.DbContext.Set<UserBook>().Find(userId, bookId);
            entity.Name = name;
            //注意：Update已由自定义的仓储重写,这里会直接调用重写的方法
            entity = _repository.Update(entity);
            _unitOfWork.SaveChanges();
            return entity;
        }
    }
}