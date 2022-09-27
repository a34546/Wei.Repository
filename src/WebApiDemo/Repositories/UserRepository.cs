using WebApiDemo.Data;
using Wei.Repository;

namespace WebApiDemo
{
    public class UserRepository : Repository<UserDbContext, User>, IUserRepository
    {
        public UserRepository(DbContextFactory dbContextFactory) : base(dbContextFactory)
        {
        }

        public override Task<User> InsertAsync(User entity, CancellationToken cancellationToken = default)
        {
            entity.Id = Guid.NewGuid().ToString();
            return base.InsertAsync(entity, cancellationToken);
        }

        public override User Update(User entity)
        {
            entity.Name += $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
            return base.Update(entity);
        }
    }

    public interface IUserRepository : IRepository<UserDbContext, User>
    {
    }
}
