using Dapper;
using Wei.Repository;

namespace WebApiDemo
{
    public static class SeedDataExtensions
    {
        public static void InitSeedData(this IApplicationBuilder app)
        {
            InitBookTable(app);
            InitUserTable(app);
        }

        private static void InitBookTable(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork<BookDbContext>>();
            using var conn = unitOfWork.GetConnection();
            var count = conn.ExecuteScalar<int>("select count(*)  from sqlite_master where type='table' and name = 'Book'");
            if (count <= 0)
            {
                conn.Execute(@"
                           CREATE TABLE ""Book"" (
	                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
	                            Name TEXT
                            );");
                conn.Execute("INSERT INTO \"Book\" (Name) VALUES\r\n\t ('C#');");
            }
        }

        private static void InitUserTable(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork<UserDbContext>>();
            using var conn = unitOfWork.GetConnection();
            var count = conn.ExecuteScalar<int>("select count(*)  from sqlite_master where type='table' and name = 'User'");
            if (count <= 0)
            {
                conn.Execute(@"
                           CREATE TABLE ""User"" (
	                            Id TEXT PRIMARY KEY,
	                            Name TEXT
                            );");
                conn.Execute("INSERT INTO \"User\" (Id,Name) VALUES\r\n\t ('1','张三');");
            }
        }
    }
}
