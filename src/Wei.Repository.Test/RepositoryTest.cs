using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions.Ordering;

namespace Wei.Repository.Test
{
    public class RepositoryTest : TestBase
    {
        [Fact, Order(1)]
        public async Task InsertAsync()
        {
            var id1 = Guid.NewGuid().ToString();
            var id2 = Guid.NewGuid().ToString();
            var id3 = Guid.NewGuid().ToString();
            var id4 = Guid.NewGuid().ToString();
            var test1 = TestRepository.Insert(new Test { Id = id1, Name = "Insert1" });
            var test2 = Repository.Insert(new Test { Id = id2, Name = "Insert2" });
            var test3 = await TestRepository.InsertAsync(new Test { Id = id3, Name = "InsertAsync1" });
            var test4 = await Repository.InsertAsync(new Test { Id = id4, Name = "InsertAsync2" });
            await UnitOfWork.SaveChangesAsync();
            Assert.NotNull(test1);
            Assert.NotNull(test2);
            Assert.NotNull(test3);
            Assert.NotNull(test4);
            Assert.Equal(test1.Id, id1);
            Assert.Equal(test2.Id, id2);
            Assert.Equal(test3.Id, id3);
            Assert.Equal(test4.Id, id4);
        }

        [Fact, Order(2)]
        public async Task InsertRangeAsync()
        {
            TestRepository.Insert(new List<Test> {
                new Test { Id = Guid.NewGuid().ToString(), Name = "InsertRange1" },
                new Test { Id = Guid.NewGuid().ToString(), Name = "InsertRange2" },
            });
            Repository.Insert(new List<Test> {
                new Test { Id = Guid.NewGuid().ToString(), Name = "InsertRange3" },
                new Test { Id = Guid.NewGuid().ToString(), Name = "InsertRange4" },
            });
            await TestRepository.InsertAsync(new List<Test> {
                new Test { Id = Guid.NewGuid().ToString(), Name = "InsertRangeAsync1" },
                new Test { Id = Guid.NewGuid().ToString(), Name = "InsertRangeAsync2" },
            });
            await Repository.InsertAsync(new List<Test> {
                new Test { Id = Guid.NewGuid().ToString(), Name = "InsertRangeAsync3" },
                new Test { Id = Guid.NewGuid().ToString(), Name = "InsertRangeAsync4" },
            });
            await UnitOfWork.SaveChangesAsync();
            var test1 = await TestRepository.GetAllAsync(x => new string[] { "InsertRange1", "InsertRange2", "InsertRange3", "InsertRange4" }.Contains(x.Name));
            var test2 = await Repository.GetAllAsync(x => new string[] { "InsertRangeAsync1", "InsertRangeAsync2", "InsertRangeAsync3", "InsertRangeAsync4" }.Contains(x.Name));

            Assert.True(test1.Count() >= 4);
            Assert.True(test2.Count() >= 4);

        }

        [Fact, Order(3)]
        public void Update()
        {
            if (!TestRepository.Any(x => x.Name == "Update1"))
            {
                TestRepository.Insert(new List<Test> {
                    new Test { Id = Guid.NewGuid().ToString(), Name = "Update1" },
                });
                UnitOfWork.SaveChanges();
            }

            var test1 = TestRepository.FirstOrDefault(x => x.Name == "Update1");
            test1.Name += "_1";
            TestRepository.Update(test1);

            if (!TestRepository.Any(x => x.Name == "Update2"))
            {
                Repository.Insert(new List<Test> {
                    new Test { Id = Guid.NewGuid().ToString(), Name = "Update2" },
                });
                UnitOfWork.SaveChanges();
            }
            var test2 = Repository.FirstOrDefault(x => x.Name == "Update2");
            test2.Name += "_1";
            Repository.Update(test2);

            UnitOfWork.SaveChanges();

            var res1 = TestRepository.Get(test1.Id);
            var res2 = Repository.Get(test2.Id);

            Assert.Equal(test1.Name, res1.Name);
            Assert.Equal(test2.Name, res2.Name);
        }

        [Fact, Order(4)]
        public async Task UpdateRange()
        {
            if (!TestRepository.Any(x => x.Name == "UpdateRange1"))
            {
                TestRepository.Insert(new List<Test> {
                    new Test { Id = Guid.NewGuid().ToString(), Name = "UpdateRange1" },
                    new Test { Id = Guid.NewGuid().ToString(), Name = "UpdateRange1" },
                });
                UnitOfWork.SaveChanges();
            }

            var update1list = TestRepository.Query(x => x.Name == "UpdateRange1").ToList();
            foreach (var item in update1list)
            {
                item.Name = "UpdateRange111";
            }
            TestRepository.Update(update1list);

            if (!Repository.Any(x => x.Name == "UpdateRange2"))
            {
                Repository.Insert(new List<Test> {
                    new Test { Id = Guid.NewGuid().ToString(), Name = "UpdateRange2" },
                    new Test { Id = Guid.NewGuid().ToString(), Name = "UpdateRange2" },
                });
                UnitOfWork.SaveChanges();
            }

            var update2list = Repository.Query(x => x.Name == "UpdateRange2").ToList();
            foreach (var item in update2list)
            {
                item.Name = "UpdateRange222";
            }
            Repository.Update(update2list);
            await UnitOfWork.SaveChangesAsync();
            var res1 = TestRepository.QueryNoTracking(x => x.Name == "UpdateRange111").ToList();
            var res2 = Repository.QueryNoTracking(x => x.Name == "UpdateRange222").ToList();

            Assert.True(res1.Count >= 2);
            Assert.True(res2.Count >= 2);

        }

        [Fact, Order(4)]
        public async Task UpdateBy()
        {
            if (!TestRepository.Any(x => x.Name == "UpdateBy"))
            {
                TestRepository.Insert(new List<Test> {
                    new Test { Id = Guid.NewGuid().ToString(), Name = "UpdateBy" },
                    new Test { Id = Guid.NewGuid().ToString(), Name = "UpdateBy" },
                });
                UnitOfWork.SaveChanges();
            }
            var name = "UpdateBy" + Guid.NewGuid();
            var entities = await TestRepository.UpdateAsync(x => x.Name == "UpdateBy", x => x.Name = name);
            await UnitOfWork.SaveChangesAsync();
            var res1 = TestRepository.QueryNoTracking(x => x.Name == name).ToList();
            Assert.True(entities.Count() == res1.Count);

        }

        [Fact, Order(5)]
        public async void Delete()
        {
            var id1 = Guid.NewGuid().ToString();
            var id2 = Guid.NewGuid().ToString();
            var id3 = Guid.NewGuid().ToString();
            var id4 = Guid.NewGuid().ToString();
            TestRepository.Insert(new List<Test> {
                new Test { Id = id1, Name = "Delete" },
                new Test { Id = id2, Name = "Delete" },
                new Test { Id = id3, Name = "Delete" },
                new Test { Id = id4, Name = "Delete" },
            });
            UnitOfWork.SaveChanges();

            TestRepository.Delete(id1);
            Repository.Delete(id2);
            TestRepository.Delete(x => x.Id == id3);
            Repository.Delete(x => x.Id == id4);
            UnitOfWork.SaveChanges();

            var res1 = TestRepository.Get(id1);
            var res2 = Repository.Get(id1);
            var res3 = await TestRepository.GetAsync(id1);
            var res4 = await Repository.GetAsync(id1);

            Assert.Null(res1);
            Assert.Null(res2);
            Assert.Null(res3);
            Assert.Null(res4);
        }

        [Fact, Order(6)]
        public async Task AnyAsync()
        {
            var res1 = Repository.Any();
            var res2 = await TestRepository.AnyAsync();

            var res3 = Repository.Any(x => x.Name.Contains("Update"));
            var res4 = await TestRepository.AnyAsync(x => x.Name.Contains("Update"));

            Assert.True(res1 && res2 && res3 && res4);
        }

        [Fact, Order(7)]
        public async Task CountAsync()
        {
            var res1 = Repository.Count();
            var res2 = await TestRepository.CountAsync();
            var res3 = Repository.Count(x => x.Name.Contains("Update"));
            var res4 = await TestRepository.CountAsync(x => x.Name.Contains("Update"));
            Assert.True(res1 > 0);
            Assert.True(res2 > 0);
            Assert.True(res3 > 0);
            Assert.True(res4 > 0);
        }

    }
}
