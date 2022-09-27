using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Wei.Repository.Test
{
    public class TestRepository : Repository<Test>, ITestRepository
    {
        public TestRepository(DbContextFactory dbContextFactory) : base(dbContextFactory)
        {
        }

        public override Test FirstOrDefault()
        {
            return null;
        }
    }

    public interface ITestRepository : IRepository<Test>
    {
    }
}
