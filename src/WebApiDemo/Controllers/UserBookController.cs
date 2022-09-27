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
        // ���Ͳִ�
        // ���DbContextʱ�����������ָ��DbContext��Ĭ��ʹ�õ�һ��ע���DbContext
        private readonly IRepository<UserBookDbContext, UserBook> _repository;

        // ������Ԫ,
        // ���������ָ��DbContext��Ĭ��ʹ�õ�һ��ע���DbContext
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
        /// ���ݸ���������ȡ
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
            //ע�⣺Update�����Զ���Ĳִ���д,�����ֱ�ӵ�����д�ķ���
            entity = _repository.Update(entity);
            _unitOfWork.SaveChanges();
            return entity;
        }
    }
}