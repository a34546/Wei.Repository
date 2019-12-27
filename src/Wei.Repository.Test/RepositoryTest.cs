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
    public class RepositoryTest
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
        public RepositoryTest()
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

        #region Insert
        [Fact, Order(1)]
        public void Insert()
        {
            var user1 = _userRepository.Insert(new User { UserName = "userRepository_Insert" });
            var user2 = _repository.Insert(new User { UserName = "repository_Insert" });
            _unitOfWork.SaveChanges();
            Assert.True(user1.Id > 0);
            Assert.True(user2.Id > 0);
        }

        [Fact, Order(1)]
        public async Task InsertAsync()
        {
            var user1 = await _userRepository.InsertAsync(new User { UserName = "userRepository_InsertAsync" });
            var user2 = await _repository.InsertAsync(new User { UserName = "repository_InsertAsync" });
            await _unitOfWork.SaveChangesAsync();
            Assert.True(user1.Id > 0);
            Assert.True(user2.Id > 0);
        }

        [Fact, Order(1)]
        public void InsertRange()
        {
            _userRepository.Insert(new List<User> {
                new User { UserName = "userRepository_InsertRange_1" },
                new User { UserName = "userRepository_InsertRange_2" },
            });
            _repository.Insert(new List<User> {
                new User { UserName = "repository_InsertRange_1" },
                new User { UserName = "repository_InsertRange_2" },
            });
            _unitOfWork.SaveChanges();
            var users1 = _userRepository.GetAll(x => x.UserName.StartsWith("userRepository_InsertRange_"));
            var users2 = _repository.GetAll(x => x.UserName.StartsWith("repository_InsertRange_"));

            Assert.True(users1.Count >= 2);
            Assert.True(users2.Count >= 2);

        }

        [Fact, Order(1)]
        public async Task InsertRangeAsync()
        {
            await _userRepository.InsertAsync(new List<User> {
                new User { UserName = "userRepository_InsertRangeAsync_1" },
                new User { UserName = "userRepository_InsertRangeAsync_2" },
            });
            await _repository.InsertAsync(new List<User> {
                new User { UserName = "repository_InsertRangeAsync_1" },
                new User { UserName = "repository_InsertRangeAsync_2" },
            });
            await _unitOfWork.SaveChangesAsync();
            var users1 = await _userRepository.GetAllAsync(x => x.UserName.StartsWith("userRepository_InsertRangeAsync_"));
            var users2 = await _repository.GetAllAsync(x => x.UserName.StartsWith("repository_InsertRangeAsync_"));

            Assert.True(users1.Count >= 2);
            Assert.True(users2.Count >= 2);

        }

        #endregion Insert

        #region Update
        [Fact, Order(2)]
        public void Update()
        {
            var user1 = _userRepository.QueryNoTracking().FirstOrDefault(x => x.UserName == "userRepository_Insert");
            user1.Mobile = "13012341234";
            _userRepository.Update(user1);

            var user2 = _repository.QueryNoTracking().FirstOrDefault(x => x.UserName == "repository_Insert");
            user2.Mobile = "13056785678";
            _userRepository.Update(user2);

            _unitOfWork.SaveChanges();

            user1 = _userRepository.Get(user1.Id);
            user2 = _repository.Get(user2.Id);

            Assert.Equal("13012341234", user1.Mobile);
            Assert.Equal("13056785678", user2.Mobile);
            Assert.NotNull(user1.UpdateTime);
            Assert.NotNull(user2.UpdateTime);
        }

        [Fact, Order(2)]
        public async Task UpdateAsync()
        {
            var user1 = _userRepository.QueryNoTracking().FirstOrDefault(x => x.UserName == "userRepository_InsertAsync");
            user1.Mobile = "13012341234";
            await _userRepository.UpdateAsync(user1);

            var user2 = _repository.QueryNoTracking().FirstOrDefault(x => x.UserName == "repository_InsertAsync");
            user2.Mobile = "13056785678";
            await _repository.UpdateAsync(user2);

            await _unitOfWork.SaveChangesAsync();

            user1 = await _userRepository.GetAsync(user1.Id);
            user2 = await _repository.GetAsync(user2.Id);

            Assert.Equal("13012341234", user1.Mobile);
            Assert.Equal("13056785678", user2.Mobile);
            Assert.NotNull(user1.UpdateTime);
            Assert.NotNull(user2.UpdateTime);

        }
        #endregion Update

        #region Delete

        [Fact, Order(3)]
        public void Delete()
        {
            var user1 = _userRepository.QueryNoTracking().FirstOrDefault(x => x.UserName == "userRepository_Insert");
            _userRepository.Delete(user1);

            var user2 = _repository.QueryNoTracking().FirstOrDefault(x => x.UserName == "repository_Insert");
            _repository.Delete(user2);

            _unitOfWork.SaveChanges();

            user1 = _userRepository.QueryNoTracking().FirstOrDefault(x => x.Id == user1.Id);
            user2 = _repository.QueryNoTracking().FirstOrDefault(x => x.Id == user2.Id);

            Assert.True(user1.IsDelete);
            Assert.True(user2.IsDelete);
            Assert.NotNull(user1.DeleteTime);
            Assert.NotNull(user2.DeleteTime);
        }

        [Fact, Order(3)]
        public async Task DeleteAsync()
        {
            var user1 = _userRepository.QueryNoTracking().FirstOrDefault(x => x.UserName == "userRepository_InsertAsync");
            await _userRepository.DeleteAsync(user1);

            var user2 = _repository.QueryNoTracking().FirstOrDefault(x => x.UserName == "repository_InsertAsync");
            await _repository.DeleteAsync(user2);

            await _unitOfWork.SaveChangesAsync();

            user1 = _userRepository.QueryNoTracking().FirstOrDefault(x => x.Id == user1.Id);
            user2 = _repository.QueryNoTracking().FirstOrDefault(x => x.Id == user2.Id);

            Assert.True(user1.IsDelete);
            Assert.True(user2.IsDelete);
            Assert.NotNull(user1.DeleteTime);
            Assert.NotNull(user2.DeleteTime);
        }

        [Fact, Order(3)]
        public void DeleteById()
        {
            var user1 = _userRepository.Insert(new User { UserName = "userRepository_DeleteById" });
            var user2 = _repository.Insert(new User { UserName = "repository_DeleteById" });
            _unitOfWork.SaveChanges();
            Assert.True(user1.Id > 0);
            Assert.True(user2.Id > 0);

            _userRepository.Delete(user1.Id);
            _repository.Delete(user2.Id);
            _unitOfWork.SaveChanges();

            user1 = _userRepository.QueryNoTracking().FirstOrDefault(x => x.Id == user1.Id);
            user2 = _repository.QueryNoTracking().FirstOrDefault(x => x.Id == user2.Id);

            Assert.True(user1.IsDelete);
            Assert.True(user2.IsDelete);
            Assert.NotNull(user1.DeleteTime);
            Assert.NotNull(user2.DeleteTime);
        }

        [Fact, Order(3)]
        public async Task DeleteByIdAsync()
        {
            var user1 = await _userRepository.InsertAsync(new User { UserName = "userRepository_DeleteByIdAsync" });
            var user2 = await _repository.InsertAsync(new User { UserName = "repository_DeleteByIdAsync" });
            await _unitOfWork.SaveChangesAsync();
            Assert.True(user1.Id > 0);
            Assert.True(user2.Id > 0);

            await _userRepository.DeleteAsync(user1.Id);
            await _repository.DeleteAsync(user2.Id);
            await _unitOfWork.SaveChangesAsync();

            user1 = _userRepository.QueryNoTracking().FirstOrDefault(x => x.Id == user1.Id);
            user2 = _repository.QueryNoTracking().FirstOrDefault(x => x.Id == user2.Id);

            Assert.True(user1.IsDelete);
            Assert.True(user2.IsDelete);
            Assert.NotNull(user1.DeleteTime);
            Assert.NotNull(user2.DeleteTime);
        }

        [Fact, Order(3)]
        public void DeleteBy()
        {
            _userRepository.Delete(x => x.UserName.StartsWith("userRepository_InsertRange_"));
            _repository.Delete(x => x.UserName.StartsWith("repository_InsertRange_"));
            _unitOfWork.SaveChanges();

            var user1 = _userRepository.QueryNoTracking(x => x.UserName.StartsWith("userRepository_InsertRange_")).ToList();
            var user2 = _repository.QueryNoTracking(x => x.UserName.StartsWith("repository_InsertRange_")).ToList();

            var isAllDelete1 = !(user1.Any(x => x.IsDelete == false));
            var isAllDelete2 = !(user2.Any(x => x.IsDelete == false));

            Assert.True(isAllDelete1);
            Assert.True(isAllDelete2);
        }

        [Fact, Order(3)]
        public async Task DeleteByAsync()
        {
            await _userRepository.DeleteAsync(x => x.UserName.StartsWith("userRepository_InsertRangeAsync_"));
            await _repository.DeleteAsync(x => x.UserName.StartsWith("repository_InsertRangeAsync_"));
            await _unitOfWork.SaveChangesAsync();

            var user1 = await _userRepository.QueryNoTracking(x => x.UserName.StartsWith("userRepository_InsertRangeAsync_")).ToListAsync();
            var user2 = await _repository.QueryNoTracking(x => x.UserName.StartsWith("repository_InsertRangeAsync_")).ToListAsync();

            var isAllDelete1 = !(user1.Any(x => x.IsDelete == false));
            var isAllDelete2 = !(user2.Any(x => x.IsDelete == false));

            Assert.True(isAllDelete1);
            Assert.True(isAllDelete2);
        }

        #endregion

        #region HardDelete

        [Fact, Order(4)]
        public void HardDelete()
        {
            var user1 = _userRepository.Insert(new User { UserName = "userRepository_HardDelete" });
            var user2 = _repository.Insert(new User { UserName = "repository_HardDelete" });
            _unitOfWork.SaveChanges();

            _userRepository.HardDelete(user1);
            _repository.HardDelete(user2);
            _unitOfWork.SaveChanges();

            user1 = _userRepository.QueryNoTracking().FirstOrDefault(x => x.Id == user1.Id);
            user2 = _repository.QueryNoTracking().FirstOrDefault(x => x.Id == user2.Id);

            Assert.Null(user1);
            Assert.Null(user2);
        }

        [Fact, Order(4)]
        public async Task HardDeleteAsync()
        {
            var user1 = await _userRepository.InsertAsync(new User { UserName = "userRepository_HardDeleteAsync" });
            var user2 = await _repository.InsertAsync(new User { UserName = "repository_HardDeleteAsync" });
            await _unitOfWork.SaveChangesAsync();

            await _userRepository.HardDeleteAsync(user1);
            await _repository.HardDeleteAsync(user2);
            await _unitOfWork.SaveChangesAsync();

            user1 = _userRepository.QueryNoTracking().FirstOrDefault(x => x.Id == user1.Id);
            user2 = _repository.QueryNoTracking().FirstOrDefault(x => x.Id == user2.Id);

            Assert.Null(user1);
            Assert.Null(user2);
        }

        [Fact, Order(4)]
        public void HardDeleteById()
        {
            var user1 = _userRepository.Insert(new User { UserName = "userRepository_HardDeleteById" });
            var user2 = _repository.Insert(new User { UserName = "repository_HardDeleteById" });
            _unitOfWork.SaveChanges();

            _userRepository.HardDelete(user1.Id);
            _repository.HardDelete(user2.Id);
            _unitOfWork.SaveChanges();

            user1 = _userRepository.QueryNoTracking().FirstOrDefault(x => x.Id == user1.Id);
            user2 = _repository.QueryNoTracking().FirstOrDefault(x => x.Id == user2.Id);

            Assert.Null(user1);
            Assert.Null(user2);
        }

        [Fact, Order(4)]
        public async Task HardDeleteByIdAsync()
        {
            var user1 = await _userRepository.InsertAsync(new User { UserName = "userRepository_HardDeleteByIdAsync" });
            var user2 = await _repository.InsertAsync(new User { UserName = "repository_HardDeleteByIdAsync" });
            await _unitOfWork.SaveChangesAsync();

            await _userRepository.HardDeleteAsync(user1.Id);
            await _repository.HardDeleteAsync(user2.Id);
            await _unitOfWork.SaveChangesAsync();

            user1 = _userRepository.QueryNoTracking().FirstOrDefault(x => x.Id == user1.Id);
            user2 = _repository.QueryNoTracking().FirstOrDefault(x => x.Id == user2.Id);

            Assert.Null(user1);
            Assert.Null(user2);
        }

        [Fact, Order(4)]
        public void HardDeleteBy()
        {
            var user1 = _userRepository.Insert(new User { UserName = "userRepository_HardDeleteById" });
            var user2 = _repository.Insert(new User { UserName = "repository_HardDeleteById" });
            _unitOfWork.SaveChanges();

            _userRepository.HardDelete(user1.Id);
            _repository.HardDelete(user2.Id);
            _unitOfWork.SaveChanges();

            user1 = _userRepository.QueryNoTracking().FirstOrDefault(x => x.Id == user1.Id);
            user2 = _repository.QueryNoTracking().FirstOrDefault(x => x.Id == user2.Id);

            Assert.Null(user1);
            Assert.Null(user2);
        }

        [Fact, Order(4)]
        public async Task HardDeleteByAsync()
        {
            await _userRepository.InsertAsync(
                    new List<User>
                    {
                        new User { UserName = "userRepository_HardDeleteByAsync_1" },
                        new User { UserName = "userRepository_HardDeleteByAsync_2" },
                    }
                );
            await _repository.InsertAsync(
                    new List<User>
                    {
                        new User { UserName = "repository_HardDeleteByAsync_1" },
                        new User { UserName = "repository_HardDeleteByAsync_2" },
                    }
                );
            await _unitOfWork.SaveChangesAsync();

            var user1 = _userRepository.QueryNoTracking().FirstOrDefault(x => x.UserName.StartsWith("userRepository_HardDeleteByAsync_"));
            var user2 = _repository.QueryNoTracking().FirstOrDefault(x => x.UserName.StartsWith("repository_HardDeleteByAsync_"));
            Assert.NotNull(user1);
            Assert.NotNull(user2);

            await _userRepository.HardDeleteAsync(x => x.UserName.StartsWith("userRepository_HardDeleteByAsync_"));
            await _repository.HardDeleteAsync(x => x.UserName.StartsWith("repository_HardDeleteByAsync_"));
            await _unitOfWork.SaveChangesAsync();

            user1 = _userRepository.QueryNoTracking().FirstOrDefault(x => x.UserName.StartsWith("userRepository_HardDeleteByAsync_"));
            user2 = _repository.QueryNoTracking().FirstOrDefault(x => x.UserName.StartsWith("repository_HardDeleteByAsync_"));

            Assert.Null(user1);
            Assert.Null(user2);
        }

        #endregion

        #region Aggregate
        [Fact, Order(5)]
        public void Any()
        {
            var hasDelete = _userRepository.Any(x => x.IsDelete);
            Assert.True(hasDelete);
        }

        [Fact, Order(5)]
        public async Task AnyAsync()
        {
            var hasDelete = await _userRepository.AnyAsync(x => x.IsDelete);
            Assert.True(hasDelete);
        }

        [Fact, Order(5)]
        public void Count()
        {
            var count = _userRepository.Count();
            Assert.True(count > 0);
        }

        [Fact, Order(5)]
        public async Task CountAsync()
        {
            var count = await _userRepository.CountAsync();
            Assert.True(count > 0);
        }

        [Fact, Order(5)]
        public void CountBy()
        {
            var count = _userRepository.Count(x => x.IsDelete);
            Assert.True(count > 0);
        }

        [Fact, Order(5)]
        public async Task CountByAsync()
        {
            var count = await _userRepository.CountAsync(x => x.IsDelete);
            Assert.True(count > 0);
        }
        [Fact, Order(5)]
        public void LongCount()
        {
            var count = _userRepository.LongCount();
            Assert.True(count > 0);
        }

        [Fact, Order(5)]
        public async Task LongCountAsync()
        {
            var count = await _userRepository.LongCountAsync();
            Assert.True(count > 0);
        }

        [Fact, Order(5)]
        public void LongCountBy()
        {
            var count = _userRepository.LongCount(x => x.IsDelete);
            Assert.True(count > 0);
        }

        [Fact, Order(5)]
        public async Task LongCountByAsync()
        {
            var count = await _userRepository.LongCountAsync(x => x.IsDelete);
            Assert.True(count > 0);
        }

        #endregion

    }
}
