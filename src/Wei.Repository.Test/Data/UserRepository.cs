using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Wei.Repository.Test
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(DbContext dbDbContext) : base(dbDbContext)
        {
        }

        public override User FirstOrDefault()
        {
            return null;
        }
    }

    public interface IUserRepository : IRepository<User>
    {
    }
}
