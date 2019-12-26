using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wei.Repository.Test
{
    public class TestTable2Repository : Repository<TestTable2>, ITestTable2Repository
    {
        public TestTable2Repository(DbContext dbDbContext) : base(dbDbContext)
        {

        }

        public override List<TestTable2> GetAll()
        {
            return null;
        }

        public override Task<TestTable2> FirstOrDefaultAsync()
        {
            return null;
        }
    }

    public interface ITestTable2Repository : IRepository<TestTable2>
    {
    }
}
