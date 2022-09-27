using Microsoft.EntityFrameworkCore;
using WebApiDemo;
using WebApiDemo.Data;
using Wei.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRepository<BookDbContext>(ops => ops.UseSqlite("Data Source=book.db"));
// 读写分离场景，也可以建两个DbContext来实现
builder.Services.AddRepository<UserDbContext>(ops => ops.UseSqlite("Data Source=user.db"));
builder.Services.AddRepository<UserReadonlyDbContext>(ops => ops.UseSqlite("Data Source=user.db"));
// 复合主键使用场景
builder.Services.AddRepository<UserBookDbContext>(ops => ops.UseSqlite("Data Source=userbook.db"));

// Mysql,SqlServer 可以安装驱动后自行测试，demo暂只用sqllite测试
//builder.Services.AddRepository<UserDbContext>(ops => ops.UseMysql("xxx"));
//builder.Services.AddRepository<UserDbContext>(ops =>ops.UseSqlServer("xxx"));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 初始化sqllite数据库
app.InitSeedData();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
