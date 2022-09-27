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
    public class DapperTest : TestBase
    {

        [Fact, Order(1)]
        public async Task DapperInsert()
        {
            var id = Guid.NewGuid().ToString();
            var test = new Test
            {
                Id = id,
                Name = nameof(DapperInsert)
            };

            await UnitOfWork.ExecuteAsync("INSERT INTO Test (Id,Name) VALUES (@Id,@Name);", test);

            var users = await UnitOfWork.QueryAsync<Test>("select * from Test where Id =@Id", new { Id = id });
            Assert.NotNull(users);
            Assert.Single(users);
            Assert.True(users.First().Name == nameof(DapperInsert));
        }

        [Fact, Order(2)]
        public async Task QueryAsync()
        {
            var users = await UnitOfWork.QueryAsync<Test>("select * from Test;");
            Assert.True(users.Any());
        }

        [Fact, Order(3)]
        public async Task Transaction_Commit()
        {
            var test1 = new Test { Id = Guid.NewGuid().ToString(), Name = "Transaction_Commit1" };
            var test2 = new Test { Id = Guid.NewGuid().ToString(), Name = "Transaction_Commit2" };
            using (var tran = UnitOfWork.BeginTransaction())
            {
                try
                {
                    await UnitOfWork.ExecuteAsync("INSERT INTO Test (Id,Name) VALUES (@Id,@Name);", test1, tran);
                    await UnitOfWork.ExecuteAsync("INSERT INTO Test (Id,Name) VALUES (@Id,@Name);", test2, tran);
                    tran.Commit();
                }
                catch (Exception)
                {
                    tran.Rollback();
                }
            }
            var res1 = UnitOfWork.GetConnection().QueryFirstOrDefault<Test>("select * from Test where Id =@Id", test1);
            var res2 = UnitOfWork.GetConnection().QueryFirstOrDefault<Test>("select * from Test where Id =@Id", test2);
            Assert.NotNull(res1);
            Assert.NotNull(res2);
            Assert.Equal(test1.Id, res1.Id);
            Assert.Equal(test2.Id, res2.Id);
        }

        [Fact, Order(4)]
        public async Task Transaction_Rollback()
        {
            var test1 = new Test { Id = Guid.NewGuid().ToString(), Name = "Transaction_Rollback1" };
            var test2 = new Test { Id = Guid.NewGuid().ToString(), Name = "Transaction_Rollback2" };
            using (var tran = UnitOfWork.BeginTransaction())
            {
                try
                {
                    await UnitOfWork.ExecuteAsync("INSERT INTO Test (Id,Name) VALUES (@Id,@Name);", test1, tran);
                    await UnitOfWork.ExecuteAsync("INSERT INTO Test (Id,Name) VALUES (@Id,@Name);", test2, tran);
                    throw new Exception("²âÊÔ»Ø¹ö");
                    tran.Commit();
                }
                catch (Exception)
                {
                    tran.Rollback();
                }
            }
            var res1 = UnitOfWork.GetConnection().QueryFirstOrDefault<Test>("select * from Test where Id =@Id", test1);
            var res2 = UnitOfWork.GetConnection().QueryFirstOrDefault<Test>("select * from Test where Id =@Id", test2);
            Assert.Null(res1);
            Assert.Null(res2);
        }

        [Fact, Order(5)]
        public async Task HybridTransaction_Commit()
        {
            var test1 = new Test { Id = Guid.NewGuid().ToString(), Name = "HybridTransaction_1" };
            var test2 = new Test { Id = Guid.NewGuid().ToString(), Name = "HybridTransaction_2" };
            using (var tran = UnitOfWork.BeginTransaction())
            {
                try
                {
                    await TestRepository.InsertAsync(test1);
                    await UnitOfWork.SaveChangesAsync();
                    await UnitOfWork.ExecuteAsync("INSERT INTO Test (Id,Name) VALUES (@Id,@Name);", test2, tran);
                    tran.Commit();
                }
                catch (Exception)
                {
                    tran.Rollback();
                }
            }
            var res1 = UnitOfWork.GetConnection().QueryFirstOrDefault<Test>("select * from Test where Id =@Id", test1);
            var res2 = UnitOfWork.GetConnection().QueryFirstOrDefault<Test>("select * from Test where Id =@Id", test2);
            Assert.NotNull(res1);
            Assert.NotNull(res2);
            Assert.Equal(test1.Id, res1.Id);
            Assert.Equal(test2.Id, res2.Id);
        }

        [Fact, Order(6)]
        public async Task HybridTransaction_Rollback()
        {
            var test1 = new Test { Id = Guid.NewGuid().ToString(), Name = "HybridTransaction_Rollback1" };
            var test2 = new Test { Id = Guid.NewGuid().ToString(), Name = "HybridTransaction_Rollback2" };
            using (var tran = UnitOfWork.BeginTransaction())
            {
                try
                {
                    await TestRepository.InsertAsync(test1);
                    await UnitOfWork.SaveChangesAsync();
                    await UnitOfWork.ExecuteAsync("INSERT INTO Test (Id,Name) VALUES (@Id,@Name);", test2, tran);
                    throw new Exception("²âÊÔ»Ø¹ö");
                    tran.Commit();
                }
                catch (Exception)
                {
                    tran.Rollback();
                }
            }
            var res1 = UnitOfWork.GetConnection().QueryFirstOrDefault<Test>("select * from Test where Id =@Id", test1);
            var res2 = UnitOfWork.GetConnection().QueryFirstOrDefault<Test>("select * from Test where Id =@Id", test2);
            Assert.Null(res1);
            Assert.Null(res2);
        }
    }
}
