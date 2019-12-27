using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Wei.Repository;

namespace WebApi.Data
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(DbContext dbDbContext) : base(dbDbContext)
        {
        }

        public override Task<User> FirstOrDefaultAsync()
        {
            return default;
        }
    }

    public interface IUserRepository : IRepository<User>
    {
    }
}
