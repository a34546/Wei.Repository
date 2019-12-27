
## 快速启动Demo

 只需要修改数据库连接字符串即可启动
```cs
public void ConfigureServices(IServiceCollection services)
{
    services.AddRepository<DemoDbContext>(ops =>
    {
        ops.UseMySql("换成自己的Mysql连接字符串");
    });
}
```