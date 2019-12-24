using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions.Ordering;

namespace Wei.Repository.Test
{
    public class RepositoryTest
    {
        readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// 泛型仓储
        /// </summary>
        readonly IRepository<TestTable1, long> _testTable1Repository;

        /// <summary>
        /// 自定义仓储
        /// </summary>
        readonly ITestTable2Repository _testTable2Repository;

        readonly ServiceProvider _serviceProvider;

        public RepositoryTest()
        {
            var services = new ServiceCollection();
            services.AddRepository(opt =>
            {
                opt.UseMySql("server = 127.0.0.1;database=demo;uid=root;password=root;");
            });
            _serviceProvider = services.BuildServiceProvider();
            _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();
            _testTable1Repository = _serviceProvider.GetRequiredService<IRepository<TestTable1, long>>();
            _testTable2Repository = _serviceProvider.GetRequiredService<ITestTable2Repository>();
            InitTestTable();
        }

        private void InitTestTable()
        {
            var conn = _unitOfWork.GetConnection();
            var testTable1 = conn.QueryFirstOrDefault<string>("SHOW TABLES LIKE 'TestTable1';");
            if (!"testtable1".Equals(testTable1, StringComparison.CurrentCultureIgnoreCase))
            {
                conn.Execute(@"
                             CREATE TABLE `testtable1` (
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


        [Fact, Order(0)]
        public void TestTable2()
        {
            var entity = new TestTable2
            {
                TestMethodName = nameof(TestTable2),
                TestResult = $"{nameof(TestTable2)} success",
            };
            _testTable2Repository.Insert(entity);
            _unitOfWork.SaveChanges();
            Assert.True(entity.Id > 0);

            entity.TestResult += " Update success";
            _testTable2Repository.Update(entity);
            _unitOfWork.SaveChanges();
            Assert.NotNull(entity.UpdateTime);

            _testTable2Repository.Delete(entity);
            _unitOfWork.SaveChanges();
            using var scope = _serviceProvider.CreateScope();
            var testTable2Repository = scope.ServiceProvider.GetRequiredService<ITestTable2Repository>();
            var newEntity = testTable2Repository.GetById(entity.Id);
            Assert.True(newEntity.IsDelete);
        }

        #region Insert
        [Fact, Order(1)]
        public void Insert()
        {
            var entity = new TestTable1
            {
                TestMethodName = nameof(Insert),
                TestResult = $"{nameof(Insert)} success",
            };
            _testTable1Repository.Insert(entity);
            _unitOfWork.SaveChanges();

            Assert.True(entity.Id > 0);
        }

        [Fact, Order(2)]
        public async Task InsertAsync()
        {
            var entity = new TestTable1
            {
                TestMethodName = nameof(InsertAsync),
                TestResult = $"{nameof(InsertAsync)} success",
            };
            await _testTable1Repository.InsertAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            Assert.True(entity.Id > 0);
        }

        [Fact, Order(3)]
        public void InsertRange()
        {
            var entities = new List<TestTable1>
            {
                new TestTable1
                {
                    TestMethodName =$"{nameof(InsertRange)}_1",
                    TestResult = $"{nameof(InsertRange)} success",
                },
                new TestTable1
                {
                    TestMethodName =$"{nameof(InsertRange)}_2",
                    TestResult = $"{nameof(InsertRange)} success",
                },
            };
            _testTable1Repository.Insert(entities);
            _unitOfWork.SaveChanges();

            Assert.DoesNotContain(entities, x => x.Id == 0);

        }

        [Fact, Order(4)]
        public async Task InsertRangeAsync()
        {
            var entities = new List<TestTable1>
            {
                new TestTable1
                {
                    TestMethodName =$"{nameof(InsertRangeAsync)}_1",
                    TestResult = $"{nameof(InsertRangeAsync)}_1 success",
                },
                new TestTable1
                {
                    TestMethodName =$"{nameof(InsertRangeAsync)}_2",
                    TestResult = $"{nameof(InsertRangeAsync)}_2 success",
                },
            };
            await _testTable1Repository.InsertAsync(entities);
            await _unitOfWork.SaveChangesAsync();

            Assert.DoesNotContain(entities, x => x.Id == 0);
        }
        #endregion

        #region Update
        [Fact, Order(5)]
        public void Update()
        {
            var entity = _testTable1Repository.QueryNoTracking.FirstOrDefault(x => "Insert".Equals(x.TestMethodName));
            entity.TestResult += " Update success";
            _testTable1Repository.Update(entity);
            _unitOfWork.SaveChanges();
            Assert.NotNull(entity.UpdateTime);
        }

        [Fact, Order(5)]
        public void UpdateRange()
        {
            var entities = _testTable1Repository.QueryNoTracking.Where(x => x.TestMethodName.StartsWith("InsertRange_")).ToList();
            entities[0].TestResult += " UpdateRange success";
            entities[1].TestResult += " UpdateRange success";
            _testTable1Repository.Update(entities);
            _unitOfWork.SaveChanges();

            Assert.NotNull(entities[0].UpdateTime);
            Assert.NotNull(entities[1].UpdateTime);
        }

        [Fact, Order(5)]
        public void UpdateProp()
        {
            var entity = _testTable1Repository.QueryNoTracking.FirstOrDefault(x => "Insert".Equals(x.TestMethodName));
            entity.TestResult += " UpdateProp success";
            entity.TestMethodName = "UpdateProp";
            _testTable1Repository.Update(entity, x => x.TestResult);//只更新TestResult字段,TestMethodName不会更新
            _unitOfWork.SaveChanges();

            using var scope = _serviceProvider.CreateScope();
            var testTable1Repository = scope.ServiceProvider.GetRequiredService<IRepository<TestTable1, long>>();
            entity = testTable1Repository.Query.FirstOrDefault(x => x.Id == entity.Id);

            Assert.Contains("UpdateProp", entity.TestResult);
            Assert.NotEqual("UpdateProp", entity.TestMethodName);
        }

        [Fact, Order(5)]
        public async Task UpdateAsync()
        {
            var entity = _testTable1Repository.QueryNoTracking.FirstOrDefault(x => "InsertAsync".Equals(x.TestMethodName));
            entity.TestResult += " UpdateAsync success";
            await _testTable1Repository.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            Assert.NotNull(entity.UpdateTime);
        }

        [Fact, Order(5)]
        public async Task UpdateRangeAsync()
        {
            var entities = _testTable1Repository.QueryNoTracking.Where(x => x.TestMethodName.StartsWith("InsertRangeAsync_")).ToList();
            entities[0].TestResult += " UpdateRangeAsync success";
            entities[1].TestResult += " UpdateRangeAsync success";
            await _testTable1Repository.UpdateAsync(entities);
            await _unitOfWork.SaveChangesAsync();

            Assert.NotNull(entities[0].UpdateTime);
            Assert.NotNull(entities[1].UpdateTime);

        }

        [Fact, Order(5)]
        public async Task UpdatePropAsync()
        {
            var entity = _testTable1Repository.QueryNoTracking.FirstOrDefault(x => "InsertAsync".Equals(x.TestMethodName));
            entity.TestResult += " UpdatePropAsync success";
            entity.TestMethodName = "UpdatePropAsync";
            await _testTable1Repository.UpdateAsync(entity, x => x.TestResult);//只更新TestResult字段,TestMethodName不会更新
            await _unitOfWork.SaveChangesAsync();

            using var scope = _serviceProvider.CreateScope();
            var testTable1Repository = scope.ServiceProvider.GetRequiredService<IRepository<TestTable1, long>>();
            entity = testTable1Repository.Query.FirstOrDefault(x => x.Id == entity.Id);

            Assert.Contains("UpdatePropAsync", entity.TestResult);
            Assert.NotEqual("UpdatePropAsync", entity.TestMethodName);
        }
        #endregion

        #region LogicDelete
        [Fact, Order(6)]
        public void Delete()
        {
            var entity = new TestTable1
            {
                TestMethodName = nameof(Delete),
                TestResult = $"{nameof(Delete)} success",
            };
            _testTable1Repository.Insert(entity);
            _unitOfWork.SaveChanges();
            _testTable1Repository.Delete(entity);
            _unitOfWork.SaveChanges();

            using var scope = _serviceProvider.CreateScope();
            var testTable1Repository = scope.ServiceProvider.GetRequiredService<IRepository<TestTable1, long>>();
            var newEntity = testTable1Repository.GetById(entity.Id);
            Assert.True(newEntity.IsDelete);
        }

        [Fact, Order(6)]
        public void DeleteById()
        {
            var entity = new TestTable1
            {
                TestMethodName = nameof(DeleteById),
                TestResult = $"{nameof(DeleteById)} success",
            };
            _testTable1Repository.Insert(entity);
            _unitOfWork.SaveChanges();
            _testTable1Repository.Delete(entity.Id);
            _unitOfWork.SaveChanges();

            using var scope = _serviceProvider.CreateScope();
            var testTable1Repository = scope.ServiceProvider.GetRequiredService<IRepository<TestTable1, long>>();
            var newEntity = testTable1Repository.GetById(entity.Id);
            Assert.True(newEntity.IsDelete);
        }

        [Fact, Order(6)]
        public void DeleteBy()
        {
            var entity = new TestTable1
            {
                TestMethodName = nameof(DeleteBy),
                TestResult = $"{nameof(DeleteBy)} success",
            };
            _testTable1Repository.Insert(entity);
            _unitOfWork.SaveChanges();
            _testTable1Repository.Delete(x => x.Id == entity.Id);
            _unitOfWork.SaveChanges();

            using var scope = _serviceProvider.CreateScope();
            var testTable1Repository = scope.ServiceProvider.GetRequiredService<IRepository<TestTable1, long>>();
            var newEntity = testTable1Repository.GetById(entity.Id);
            Assert.True(newEntity.IsDelete);
        }

        [Fact, Order(6)]
        public async Task DeleteAsync()
        {
            var entity = new TestTable1
            {
                TestMethodName = nameof(DeleteAsync),
                TestResult = $"{nameof(DeleteAsync)} success",
            };
            await _testTable1Repository.InsertAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            await _testTable1Repository.DeleteAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            using var scope = _serviceProvider.CreateScope();
            var testTable1Repository = scope.ServiceProvider.GetRequiredService<IRepository<TestTable1, long>>();
            var newEntity = testTable1Repository.GetById(entity.Id);
            Assert.True(newEntity.IsDelete);
        }

        [Fact, Order(6)]
        public async Task DeleteByIdAsync()
        {
            var entity = new TestTable1
            {
                TestMethodName = nameof(DeleteByIdAsync),
                TestResult = $"{nameof(DeleteByIdAsync)} success",
            };
            await _testTable1Repository.InsertAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            await _testTable1Repository.DeleteAsync(entity.Id);
            await _unitOfWork.SaveChangesAsync();

            using var scope = _serviceProvider.CreateScope();
            var testTable1Repository = scope.ServiceProvider.GetRequiredService<IRepository<TestTable1, long>>();
            var newEntity = testTable1Repository.GetById(entity.Id);
            Assert.True(newEntity.IsDelete);
        }

        [Fact, Order(6)]
        public async Task DeleteByAsync()
        {
            var entity = new TestTable1
            {
                TestMethodName = nameof(DeleteByAsync),
                TestResult = $"{nameof(DeleteByAsync)} success",
            };
            await _testTable1Repository.InsertAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            await _testTable1Repository.DeleteAsync(x => x.Id == entity.Id);
            await _unitOfWork.SaveChangesAsync();

            using var scope = _serviceProvider.CreateScope();
            var testTable1Repository = scope.ServiceProvider.GetRequiredService<IRepository<TestTable1, long>>();
            var newEntity = testTable1Repository.GetById(entity.Id);
            Assert.True(newEntity.IsDelete);
        }
        #endregion

        #region HardDelete
        [Fact, Order(6)]
        public void HardDelete()
        {
            var entity = new TestTable1
            {
                TestMethodName = nameof(HardDelete),
                TestResult = $"{nameof(HardDelete)} success",
            };
            _testTable1Repository.Insert(entity);
            _unitOfWork.SaveChanges();
            _testTable1Repository.HardDelete(entity);
            _unitOfWork.SaveChanges();

            using var scope = _serviceProvider.CreateScope();
            var testTable1Repository = scope.ServiceProvider.GetRequiredService<IRepository<TestTable1, long>>();
            var newEntity = testTable1Repository.GetById(entity.Id);
            Assert.Null(newEntity);
        }

        [Fact, Order(6)]
        public void HardDeleteById()
        {
            var entity = new TestTable1
            {
                TestMethodName = nameof(HardDeleteById),
                TestResult = $"{nameof(HardDeleteById)} success",
            };
            _testTable1Repository.Insert(entity);
            _unitOfWork.SaveChanges();
            _testTable1Repository.HardDelete(entity.Id);
            _unitOfWork.SaveChanges();

            using var scope = _serviceProvider.CreateScope();
            var testTable1Repository = scope.ServiceProvider.GetRequiredService<IRepository<TestTable1, long>>();
            var newEntity = testTable1Repository.GetById(entity.Id);
            Assert.Null(newEntity);
        }

        [Fact, Order(6)]
        public void HardDeleteBy()
        {
            var entity = new TestTable1
            {
                TestMethodName = nameof(HardDeleteBy),
                TestResult = $"{nameof(HardDeleteBy)} success",
            };
            _testTable1Repository.Insert(entity);
            _unitOfWork.SaveChanges();
            _testTable1Repository.HardDelete(x => x.Id == entity.Id);
            _unitOfWork.SaveChanges();

            using var scope = _serviceProvider.CreateScope();
            var testTable1Repository = scope.ServiceProvider.GetRequiredService<IRepository<TestTable1, long>>();
            var newEntity = testTable1Repository.GetById(entity.Id);
            Assert.Null(newEntity);
        }

        [Fact, Order(6)]
        public async Task HardDeleteAsync()
        {
            var entity = new TestTable1
            {
                TestMethodName = nameof(HardDeleteAsync),
                TestResult = $"{nameof(HardDeleteAsync)} success",
            };
            await _testTable1Repository.InsertAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            await _testTable1Repository.HardDeleteAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            using var scope = _serviceProvider.CreateScope();
            var testTable1Repository = scope.ServiceProvider.GetRequiredService<IRepository<TestTable1, long>>();
            var newEntity = testTable1Repository.GetById(entity.Id);
            Assert.Null(newEntity);
        }

        [Fact, Order(6)]
        public async Task HardDeleteByIdAsync()
        {
            var entity = new TestTable1
            {
                TestMethodName = nameof(HardDeleteByIdAsync),
                TestResult = $"{nameof(HardDeleteByIdAsync)} success",
            };
            await _testTable1Repository.InsertAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            await _testTable1Repository.HardDeleteAsync(entity.Id);
            await _unitOfWork.SaveChangesAsync();

            using var scope = _serviceProvider.CreateScope();
            var testTable1Repository = scope.ServiceProvider.GetRequiredService<IRepository<TestTable1, long>>();
            var newEntity = testTable1Repository.GetById(entity.Id);
            Assert.Null(newEntity);
        }

        [Fact, Order(6)]
        public async Task HardDeleteByAsync()
        {
            var entity = new TestTable1
            {
                TestMethodName = nameof(HardDeleteByAsync),
                TestResult = $"{nameof(HardDeleteByAsync)} success",
            };
            await _testTable1Repository.InsertAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            await _testTable1Repository.HardDeleteAsync(x => x.Id == entity.Id);
            await _unitOfWork.SaveChangesAsync();

            using var scope = _serviceProvider.CreateScope();
            var testTable1Repository = scope.ServiceProvider.GetRequiredService<IRepository<TestTable1, long>>();
            var newEntity = testTable1Repository.GetById(entity.Id);
            Assert.Null(newEntity);
        }
        #endregion

    }
}
