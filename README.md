# Wei.Repository
基于EFCore3.0+Dapper 封装Repository，实现UnitOfWork,提供基本的CURD操作，可直接注入泛型Repository，也可以继承Repository，重写CURD操作

## 快速开始

> Nuget引用包：Wei.Repository

1. 实体对象需要继承Entity
```cs
public class User : Entity
{
	[StringLength(50)]
    public string Name { get; set; }
	[StringLength(50)]
	public string Mobile { get; set; }
}
```
2. DbContext继承UnitOfWorkDbContext,如果不需要DbContext，可以忽略该步骤
```cs
public class DemoDbContext : UnitOfWorkDbContext
{
    public ChimpDbContext(DbContextOptions options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        //your code
    }
}
```
3. 注入服务,修改Startup.cs,添加AddRepository
```cs
public void ConfigureServices(IServiceCollection services)
        {
            ...
            services.AddRepository(ops =>
            {
                ops.UseMySql("");
            });
            services.AddControllers();
            ...
        }
```
4.  用泛型Repository在Controller中使用
```cs
public class HomeController : ControllerBase
{
    readonly IRepository<User> _userRepository;
	readonly IUnitOfWork _unitOfWork;
    public HomeController(IRepository<User> userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
		_unitOfWork = unitOfWork;
    }
}
```
5.  如不用泛型Repository注入，可以自定义Repository,需要继承Repository,IRepository,可以重写基类CURD方法
```cs
 public interface IUserRepository : IRepository<User>
 {
 }
 
 public class UserRepository : Repository<User>, IUserRepository
 {
    public override User GetById(int id)
    {
        return base.GetById(id);
    }
 }
 
 public class HomeController : ControllerBase
 {
    readonly IUserRepository _userRepository;
	readonly IUnitOfWork _unitOfWork;
    public HomeController(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
		_unitOfWork = unitOfWork;
    }
 }
```
注意，注入时需要注入IUserRepository类型，不能用IRepository<User>,否则不会调用重写的方法

## 详细介绍
**1. ITrack接口**
```cs
public interface ITrack
{

	/// <summary>
	/// 创建时间
	/// </summary>
	DateTime CreateTime { get; set; }

	/// <summary>
	/// 更新时间
	/// </summary>
	DateTime? UpdateTime { get; set; }

	/// <summary>
	/// 是否删除
	/// </summary>
	bool IsDelete { get; set; }

	/// <summary>
	/// 删除时间
	/// </summary>
	DateTime? DeleteTime { get; set; }
}
```
Entity实现ITrack接口，用于记录CURD操作时间
实体类继承Entity基类后，
- 对实体进行Insert操作会自动记录CreateTime，
- 进行Update操作会自动记录UpdateTime，
- 进行Delete操作时，不会真正删除，会修改IsDelete字段，标记为1，并记录DeleteTime，如需彻底删除需要调用HardDelete方法

**2. 新增接口**
```cs
_userRepository.Insert(new User());
_userRepository.Insert(new List<User>{new User()});//批量新增
_unitOfWork.SaveChanges();

//异步
//await _userRepository.InsertAsync(new User());
//await _userRepository.InsertAsync(new List<User>{new User()});
//await _unitOfWork.SaveChangesAsync();
```
**3. 更新接口**
```cs
var user =_userRepository.GetById(id);
user.Name = "TestName";
_userRepository.Update(user);
_userRepository.Update(new List<User>{user});//批量更新
_userRepository.Update(new User{Id=id,Name="TestName"},x=>x.Name);//更新指定字段
_unitOfWork.SaveChanges();

//异步
...
```
**4. 逻辑删除接口,标记IsDelete=1**
```cs
_userRepository.Delete(id);
var user =_userRepository.GetById(id);
_userRepository.Delete(user);
_userRepository.Delete(new List<User>{user});//批量删除
_userRepository.Delete(x=>x.Id == id);
_unitOfWork.SaveChanges();

//异步
...
```
**5. 物理删除接口，真正从数据库中删除**
```cs
_userRepository.HardDelete(id);
var user =_userRepository.GetById(id);
_userRepository.HardDelete(user);
_userRepository.HardDelete(new List<User>{user});//批量删除
_userRepository.HardDelete(x=>x.Id == id);
_unitOfWork.SaveChanges();

//异步
...
```
**6. 查询接口**
```cs
//根据主键查询
_userRepository.GetById(id);

//条件查询，会跟踪实体变化，如果实体有修改，SaveChanges时会自动更新修改过的字段
_userRepository.Query.First(x=>x.Id == id);

//条件查询，不跟踪实体变化,只做查询，性能快
_userRepository.QueryNoTracking.First(x=>x.Id == id);

//查询所有，IsDelete ==1的不会查询出来
_userRepository.GetAllList();

//根据条件查询所有，IsDelete ==1的不会查询出来
_userRepository.GetAllList(x=>x.Id > id);

//分页查询
_userRepository.QueryNoTracking.ToPagedList(1,10);

//Sql查询
_unitOfWork.QueryAsync<User>("select * from user");

//Sql分页查询
_unitOfWork.QueryPagedListAsync<User>(1, 10, "select * from user order by id");

```
**7. 事务**
```cs
//工作单元模式使用事务
await _userRepository.InsertAsync(user1);
await _userRepository.InsertAsync(user2);
await _unitOfWork.SaveChangesAsync();
```
```cs
//dapper使用事务
using (var tran = _unitOfWork.BeginTransaction())
{
    try
    {
        await _unitOfWork.ExecuteAsync("insert user(id,name) values(@Id,@Name)",user1,tran);
        await _unitOfWork.ExecuteAsync("insert user(id,name) values(@Id,@Name)",user1,tran);
        tran.Commit();
    }
    catch (Exception e)
    {
        tran.Rollback();
    }
}
```
```cs
//dapper+ef混合使用事务
using (var tran = _unitOfWork.BeginTransaction())
{
    try
    {
        await _userRepository.InsertAsync(user1);
        await _unitOfWork.SaveChangesAsync();

        await _unitOfWork.ExecuteAsync("insert user(id,name) values(@Id,@Name)",
            user2);
        tran.Commit();
    }
    catch (Exception e)
    {
        tran.Rollback();
    }
}
```
**8. 获GetConnection**
```cs
//通过GetConnection可以使用更多dapper扩展的方法
await _unitOfWork.GetConnection().QueryAsync("select * from user");
```

> 本项目有参考[chimp](https://github.com/longxianghui/chimp)
