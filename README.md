# Wei.Repository
基于EFCore3.0+Dapper 封装Repository，实现UnitOfWork,提供基本的CURD操作，可直接注入泛型Repository，也可以继承Repository，重写CURD操作

## 快速开始

> Nuget引用包：Wei.Repository

1. 实体对象需要继承Entity
```cs
public class User : Entity
{
public string UserName { get; set; }
public string Password { get; set; }
public string Mobile { get; set; }
}
```
2. 【可选】继承BaseDbContext,如果不需要DbContext，可以忽略该步骤
```cs
public class DemoDbContext : DbContext
{
    public DemoDbContext(DbContextOptions options) : base(options)
    {
    }
}
```
3. 注入服务,修改Startup.cs,添加AddRepository
```cs
public void ConfigureServices(IServiceCollection services)
{
    ...
    services.AddRepository<DemoDbContext>(ops =>
    {
	ops.UseMySql("server = 127.0.0.1;database=demo;uid=root;password=root;");
    });
    //services.AddRepository(ops =>
    //{
    //    ops.UseMySql("server = 127.0.0.1;database=demo;uid=root;password=root;");
    //});
    services.AddControllers();
    ...
}
```
4.  【可选】如果不用泛型Repository注入,可以自定义Repository,需要继承Repository,IRepository,可以重写基类CURD方法
```cs
public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(DbContext dbDbContext) : base(dbDbContext)
    {
    }    
    public override Task<User> FirstOrDefaultAsync()
    {
        return default;
    }
}

public interface IUserRepository : IRepository<User>
{
}
```
4.  在Controller中使用
```cs
public class UserController : ControllerBase
{
    /// <summary>
    /// 泛型注入
    /// </summary>
    private readonly IRepository<User> _repository;
    
    /// <summary>
    /// 自定义UserRepository
    /// </summary>
    private readonly IUserRepository _userRepository;
    public UserController(IRepository<User> repository,
        IUserRepository userRepository)
    {
        _repository = repository;
        _userRepository = userRepository;
    }
    
    [HttpGet]
    public async Task<User> Get()
    {
        //泛型注入不会调用重写的方法
        return await _repository.FirstOrDefaultAsync();
    
        //会调用重写的FirstOrDefaultAsync()
        //return await _userRepository.FirstOrDefaultAsync();
    }
}
```

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
实体类继承Entity,Entity实现ITrack接口，用于记录CURD操作时间，
- 对实体进行Insert操作会自动记录CreateTime，
- 进行Update操作会自动记录UpdateTime，
- 进行Delete操作时，不会真正删除，会修改IsDelete字段，标记为1，并记录DeleteTime，如需彻底删除需要调用HardDelete方法

**2. IRepository接口**
```cs
 #region Query

/// <summary>
/// 查询
/// </summary>
IQueryable<TEntity> Query();
IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate);

/// <summary>
/// 查询不跟踪实体变化
/// </summary>
IQueryable<TEntity> QueryNoTracking();
IQueryable<TEntity> QueryNoTracking(Expression<Func<TEntity, bool>> predicate);

/// <summary>
/// 根据主键获取
/// </summary>
TEntity Get(TPrimaryKey id);
Task<TEntity> GetAsync(TPrimaryKey id);

/// <summary>
/// 获取所有,默认过滤IsDelete=1的
/// </summary>
List<TEntity> GetAll();
Task<List<TEntity>> GetAllAsync();
List<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate);
Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate);

/// <summary>
/// 获取第一个
/// </summary>
TEntity FirstOrDefault();
Task<TEntity> FirstOrDefaultAsync();
TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);
Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

#endregion

#region Insert

/// <summary>
/// 新增
/// </summary>
TEntity Insert(TEntity entity);
Task<TEntity> InsertAsync(TEntity entity);

/// <summary>
/// 批量新增
/// </summary>
/// <param name="entities"></param>
void Insert(List<TEntity> entities);
Task InsertAsync(List<TEntity> entities);

#endregion Insert

#region Update

/// <summary>
/// 更新
/// </summary>
TEntity Update(TEntity entity);
Task<TEntity> UpdateAsync(TEntity entity);

#endregion Update

#region Delete

/// <summary>
/// 逻辑删除，标记IsDelete = 1
/// </summary>
/// <param name="entity"></param>
void Delete(TEntity entity);
Task DeleteAsync(TEntity entity);
void Delete(TPrimaryKey id);
Task DeleteAsync(TPrimaryKey id);
void Delete(Expression<Func<TEntity, bool>> predicate);
Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);

#endregion

#region HardDelete

/// <summary>
/// 物理删除，从数据库中移除
/// </summary>
/// <param name="entity"></param>
void HardDelete(TEntity entity);
Task HardDeleteAsync(TEntity entity);
void HardDelete(TPrimaryKey id);
Task HardDeleteAsync(TPrimaryKey id);
void HardDelete(Expression<Func<TEntity, bool>> predicate);
Task HardDeleteAsync(Expression<Func<TEntity, bool>> predicate);

#endregion

#region Aggregate

/// <summary>
/// 聚合操作
/// </summary>
bool Any(Expression<Func<TEntity, bool>> predicate);
Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);
int Count();
Task<int> CountAsync();
int Count(Expression<Func<TEntity, bool>> predicate);
Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
long LongCount();
Task<long> LongCountAsync();
long LongCount(Expression<Func<TEntity, bool>> predicate);
Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate);

#endregion
```
**3. EF工作单元事务**
```cs
await _userRepository.InsertAsync(user1);
await _userRepository.InsertAsync(user2);
await _unitOfWork.SaveChangesAsync();
```
**4. Dapper事务**
```cs
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
```
**5. EF+Dapper混合事务**
```cs
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
```
**6. 获GetConnection,使用更多dapper扩展的方法**
```cs
await _unitOfWork.GetConnection().QueryAsync("select * from user");
```

> 本项目有参考[chimp](https://github.com/longxianghui/chimp)
