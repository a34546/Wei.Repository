using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions.Ordering;

namespace Wei.Repository.Test
{
    public class DapperTest
    {
        readonly ServiceProvider _serviceProvider;
        readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// 自定义AppService
        /// </summary>
        readonly IUserRepository _userRepository;

        /// <summary>
        /// 泛型AppService
        /// </summary>
        readonly IRepository<User> _repository;
        public DapperTest()
        {
            var services = new ServiceCollection();
            services.AddRepository(opt =>
            {
                opt.UseMySql("server = 127.0.0.1;database=demo;uid=root;password=root;");
            });
            _serviceProvider = services.BuildServiceProvider();
            _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();

            _userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
            _repository = _serviceProvider.GetRequiredService<IRepository<User>>();
            InitUserTable();
        }

        private void InitUserTable()
        {
            using var scope = _serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            using var conn = unitOfWork.GetConnection();
            var userTable = conn.QueryFirstOrDefault<string>("SHOW TABLES LIKE 'User';");
            if (!"User".Equals(userTable, StringComparison.CurrentCultureIgnoreCase))
            {
                conn.Execute(@"
                            CREATE TABLE `user` (
                              `Id` int(11) NOT NULL AUTO_INCREMENT,
                              `CreateTime` datetime(6) NOT NULL,
                              `UpdateTime` datetime(6) DEFAULT NULL,
                              `IsDelete` tinyint(1) NOT NULL,
                              `DeleteTime` datetime(6) DEFAULT NULL,
                              `UserName` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
                              `Password` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
                              `Mobile` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
                              PRIMARY KEY (`Id`)
                            ) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
                    ");
            }
        }

        private void InitUsers()
        {
            if (_repository.Count(x => x.IsDelete == false) <= 10)
            {
                var users = new List<User>();
                for (int i = 0; i < 20; i++)
                {
                    users.Add(new User
                    {
                        UserName = "InitUsers_" + i
                    });
                }
                _repository.Insert(users);
                _unitOfWork.SaveChanges();
            }
        }


        [Fact, Order(1)]
        public async Task DapperInsert()
        {
            var user = new User
            {
                UserName = "DapperInsert"
            };

            await _unitOfWork.ExecuteAsync("INSERT INTO `user` (`CreateTime`, `IsDelete`, `UserName`) VALUES (now(), 0, @UserName);", user);

            var users = await _unitOfWork.QueryAsync<User>("select * from user where UserName =@UserName", user);
            Assert.True(users.First().Id > 0);
            Assert.True(user.UserName == users.First().UserName);
        }

        [Fact, Order(2)]
        public async Task QueryAsync()
        {
            InitUsers();
            var users = await _unitOfWork.QueryAsync<User>("select * from user;");
            Assert.True(users.Any());
        }

        [Fact, Order(2)]
        public async Task QueryPagedAsync()
        {
            InitUsers();
            var pageResult = await _unitOfWork.QueryPagedAsync<User>(1, 10, "select * from user where isDelete = @isDelete order by id desc", new { isDelete = 0 });
            Assert.NotNull(pageResult);
            Assert.True(pageResult.Total >= 10);
        }

        [Fact, Order(3)]
        public async Task Transaction()
        {
            var user1 = new User { UserName = "Transaction_1" };
            var user2 = new User { UserName = "Transaction_2" };
            using (var tran = _unitOfWork.BeginTransaction())
            {
                try
                {
                    await _unitOfWork.ExecuteAsync("INSERT INTO `user` (`CreateTime`, `IsDelete`, `UserName`) VALUES (now(), 0, @UserName);", user1, tran);
                    await _unitOfWork.ExecuteAsync("INSERT INTO `user` (`CreateTime`, `IsDelete`, `UserName`) VALUES (now(), 0, @UserName);", user2, tran);
                    throw new Exception();
                    tran.Commit();
                }
                catch (Exception e)
                {
                    tran.Rollback();
                }
            }
            var findUser1 = _unitOfWork.GetConnection().QueryFirstOrDefault<User>("select * from user where UserName =@UserName", user1);
            var findUser2 = _unitOfWork.GetConnection().QueryFirstOrDefault<User>("select * from user where UserName =@UserName", user2);
            Assert.Null(findUser1);
            Assert.Null(findUser2);
        }

        [Fact, Order(3)]
        public async Task HybridTransaction()
        {
            var user1 = new User { UserName = "HybridTransaction_1" };
            var user2 = new User { UserName = "HybridTransaction_2" };
            using (var tran = _unitOfWork.BeginTransaction())
            {
                try
                {
                    await _userRepository.InsertAsync(user1);
                    await _unitOfWork.ExecuteAsync("INSERT INTO `user` (`CreateTime`, `IsDelete`, `UserName`) VALUES (now(), 0, @UserName);", user2, tran);
                    throw new Exception();
                    tran.Commit();
                }
                catch (Exception e)
                {
                    tran.Rollback();
                }
            }
            var findUser1 = _unitOfWork.GetConnection().QueryFirstOrDefault<User>("select * from user where UserName =@UserName", user1);
            var findUser2 = _unitOfWork.GetConnection().QueryFirstOrDefault<User>("select * from user where UserName =@UserName", user2);
            Assert.Null(findUser1);
            Assert.Null(findUser2);
        }

    }
}
