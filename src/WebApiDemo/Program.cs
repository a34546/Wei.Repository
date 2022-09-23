using Microsoft.EntityFrameworkCore;
using WebApiDemo;
using WebApiDemo.Data;
using Wei.Repository;

var builder = WebApplication.CreateBuilder(args);

// 多DbContext使用场景

builder.Services.AddRepository<BookDbContext>(ops =>
{
    var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "book.db");
    if (!File.Exists(dbPath)) File.Create(dbPath);

    ops.UseSqlite("Data Source=book.db");
});

builder.Services.AddRepository<UserDbContext>(ops =>
{
    var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "user.db");
    if (!File.Exists(dbPath)) File.Create(dbPath);

    ops.UseSqlite("Data Source=user.db");
});

//builder.Services.AddRepository<UserDbContext>(ops =>
//{
//    ops.UseMysql("xxx");
//});

//builder.Services.AddRepository<UserDbContext>(ops =>
//{
//    ops.UseSqlServer("xxx");
//});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.InitSeedData();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
