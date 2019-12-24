using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Wei.Repository.Test
{
    public class DapperTest
    {
        readonly IUnitOfWork _unitOfWork;
        readonly IRepository<TestTable2> _testTable2Repository;

        public DapperTest()
        {
            var services = new ServiceCollection();
            services.AddRepository(opt =>
            {
                opt.UseMySql("server = 127.0.0.1;database=demo;uid=root;password=root;");
            });
            var serviceProvider = services.BuildServiceProvider();
            _unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
            _testTable2Repository = serviceProvider.GetRequiredService<IRepository<TestTable2>>();
            EnsureTestTableExists();
        }

        private void EnsureTestTableExists()
        {
            var conn = _unitOfWork.GetConnection();
            var testTable1 = conn.QueryFirstOrDefault<string>("SHOW TABLES LIKE 'TestTable1';");
            if (!"testtable1".Equals(testTable1, StringComparison.CurrentCultureIgnoreCase))
            {
                conn.Execute(@"
                            CREATE TABLE `testtable1` (
                              `Id` bigint(20) NOT NULL AUTO_INCREMENT,
                              `TestMethodName` varchar(200)  DEFAULT NULL,
                              `TestResult` varchar(200)  DEFAULT NULL,
                              `CreateTime` datetime(6) NOT NULL,
                              `UpdateTime` datetime(6) DEFAULT NULL,
                              `IsDelete` bit(1) NOT NULL,
                              `DeleteTime` datetime(6) DEFAULT NULL,
                              PRIMARY KEY (`Id`)
                            ) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
                    ");
            }
            var testTable2 = conn.QueryFirstOrDefault<string>("SHOW TABLES LIKE 'TestTable2';");
            if (!"testtable2".Equals(testTable2, StringComparison.CurrentCultureIgnoreCase))
            {
                conn.Execute(@"
                             CREATE TABLE `testtable2` (
                              `Id` int(11) NOT NULL AUTO_INCREMENT,
                              `TestMethodName` varchar(200)  DEFAULT NULL,
                              `TestResult` varchar(200)  DEFAULT NULL,
                              `CreateTime` datetime(6) NOT NULL,
                              `UpdateTime` datetime(6) DEFAULT NULL,
                              `IsDelete` bit(1) NOT NULL,
                              `DeleteTime` datetime(6) DEFAULT NULL,
                              PRIMARY KEY (`Id`)
                            ) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
                    ");
            }
        }

        [Fact]
        public async Task QueryPagedAsync()
        {
            if (await _testTable2Repository.QueryNoTracking.CountAsync() <= 10)
            {
                var posts = new List<TestTable2>();
                for (int i = 0; i < 20; i++)
                {
                    posts.Add(new TestTable2
                    {
                        TestMethodName = "QueryPagedAsync" + i,
                        TestResult = "Insert Success" + i,
                    });
                }
                await _testTable2Repository.InsertAsync(posts);
                _unitOfWork.SaveChanges();
            }
            var pageResult = await _unitOfWork.QueryPagedAsync<TestTable2>(1, 10, "select * from TestTable2 where isDelete = @isDelete order by id desc", new { isDelete = 0 });
            Assert.NotNull(pageResult);
        }

        [Fact]
        public void GetById()
        {
            //注意这里不会调用TestTable2Repository重写的GetById，需要注入 ITestTable2Repository 才能调用重写的GetById方法
            var entity = _testTable2Repository.GetById(1);
            Assert.True(true);
        }
    }
}
