using Microsoft.EntityFrameworkCore;
using WebApiDemo;
using WebApiDemo.Data;
using Wei.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRepository<BookDbContext>(ops => ops.UseSqlite("Data Source=book.db"));
// ��д���볡����Ҳ���Խ�����DbContext��ʵ��
builder.Services.AddRepository<UserDbContext>(ops => ops.UseSqlite("Data Source=user.db"));
builder.Services.AddRepository<UserReadonlyDbContext>(ops => ops.UseSqlite("Data Source=user.db"));
// ��������ʹ�ó���
builder.Services.AddRepository<UserBookDbContext>(ops => ops.UseSqlite("Data Source=userbook.db"));

// Mysql,SqlServer ���԰�װ���������в��ԣ�demo��ֻ��sqllite����
//builder.Services.AddRepository<UserDbContext>(ops => ops.UseMysql("xxx"));
//builder.Services.AddRepository<UserDbContext>(ops =>ops.UseSqlServer("xxx"));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ��ʼ��sqllite���ݿ�
app.InitSeedData();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
