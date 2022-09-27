using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions.Ordering;

namespace Wei.Repository.Test
{
    public class TestBase
    {
        readonly ServiceProvider _serviceProvider;
        public IUnitOfWork UnitOfWork { get; set; }
        public ITestRepository TestRepository { get; set; }
        public IRepository<Test> Repository { get; set; }
        public TestBase()
        {
            var services = new ServiceCollection();
            services.AddRepository<TestDbContext>(ops => ops.UseSqlite("Data Source=user.db"));

            _serviceProvider = services.BuildServiceProvider();
            UnitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();
            TestRepository = _serviceProvider.GetRequiredService<ITestRepository>();
            Repository = _serviceProvider.GetRequiredService<IRepository<Test>>();
            InitUserTable();
        }

        private void InitUserTable()
        {
            var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.db");
            if (!File.Exists(dbPath)) File.Create(dbPath);
            var unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork<TestDbContext>>();
            using var conn = unitOfWork.GetConnection();
            var count = conn.ExecuteScalar<int>("select count(*)  from sqlite_master where type='table' and name = 'Test'");
            if (count <= 0)
            {
                conn.Execute(@"
                           CREATE TABLE Test (
	                            Id TEXT PRIMARY KEY,
	                            Name TEXT
                            );");
            }
        }


    }
}
