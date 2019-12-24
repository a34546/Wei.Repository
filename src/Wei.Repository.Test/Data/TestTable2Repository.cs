using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wei.Repository.Test
{
    public class TestTable2Repository : Repository<TestTable2>, ITestTable2Repository
    {
        public TestTable2Repository(UnitOfWorkDbContext dbDbContext) : base(dbDbContext)
        {

        }

        public override TestTable2 GetById(int id)
        {
            return QueryNoTracking.FirstOrDefault(x => x.Id == id);
        }
    }

    public interface ITestTable2Repository : IRepository<TestTable2>
    {
    }
}
