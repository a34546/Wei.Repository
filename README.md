# Wei.Repository
基于.Net6平台,EFCore+Dapper 封装Repository，实现UnitOfWork,提供基本的CURD操作，支持多数据，多DbContext


## 快速开始

> Nuget引用包：Wei.Repository

1. 定义实体类
```cs
 public class User
{
    // 字段名为Id时，可省略主键标记
    //[Key]  
    public string Id { get; set; }
    public string Name { get; set; }
}
```
2. 自定义DbContext,需要继承BaseDbContext
```cs
public class UserDbContext : BaseDbContext
{
    public DemoDbContext(DbContextOptions<UserDbContext> options) 
    : base(options)
    {
    }
    public DbSet<User> User { get; set; }
}
```
3. 注入服务,添加Repository组件
```cs
builder.Services.AddRepository<UserDbContext>(ops => ops.UseSqlite("Data Source=user.db"));
// Mysql,SqlServer 可以安装驱动后自行测试，demo暂只用sqllite测试
//builder.Services.AddRepository<UserDbContext>(ops => ops.UseMysql("xxx"));
//builder.Services.AddRepository<UserDbContext>(ops =>ops.UseSqlServer("xxx"));
```

4.  【可选】自定义Repository,实现自己的业务逻辑,如果只是简单的crud,可以直接用泛型Repository
```cs
// 如果只有一个DbContext,可以不用指定UserDbContext类型
// public class UserRepository : Repository<User>, IUserRepository

public class UserRepository : Repository<UserDbContext, User>, IUserRepository
{
    public UserRepository(DbContextFactory dbContextFactory) : base(dbContextFactory)
    {
    }

    // 重写基类新增方法，实现自己的业务逻辑
    public override Task<User> InsertAsync(User entity, CancellationToken cancellationToken = default)
    {
        entity.Id = Guid.NewGuid().ToString();
        return base.InsertAsync(entity, cancellationToken);
    }
}

public interface IUserRepository : IRepository<UserDbContext, User>
{
}
```

5.  在Controller中使用
```cs
public class UserController : ControllerBase
{

    //自定义仓储
    private readonly IUserRepository _userRepository;

    // 工作单元,
    // 如果不传入指定DbContext，默认使用第一个注入的DbContext
    private readonly IUnitOfWork<UserDbContext> _unitOfWork;


    public UserController(
        IUserRepository userRepository,
        IUnitOfWork<UserDbContext> unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }
    
    [HttpPost]
    public async Task<User> InsertAsync(string name, CancellationToken cancellationToken)
    {
        var entity = await _userRepository.InsertAsync(new User { Name = name }, cancellationToken);
        _unitOfWork.SaveChanges();
        return entity;
    }
}
```

## 详细介绍

**1. 泛型IRepository接口**
```cs
    #region Query
    // 查询
    IQueryable<TEntity> Query();
    IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate);
    IQueryable<TEntity> QueryNoTracking();
    IQueryable<TEntity> QueryNoTracking(Expression<Func<TEntity, bool>> predicate);
    // 根据主键获取(支持复合主键)
    TEntity Get(params object[] id);
    ValueTask<TEntity> GetAsync(params object[] id);
    ValueTask<TEntity> GetAsync(object[] ids, CancellationToken cancellationToken);
    // 获取所有
    IEnumerable<TEntity> GetAll();
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate);
    Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    // 获取第一个或默认值
    TEntity FirstOrDefault();
    Task<TEntity> FirstOrDefaultAsync(CancellationToken cancellationToken = default);
    TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);
    Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    #endregion

    #region Insert
    // 新增
    TEntity Insert(TEntity entity);
    Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default);
    // 批量新增
    void Insert(IEnumerable<TEntity> entities);
    Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    #endregion Insert

    #region Update
    // 更新
    TEntity Update(TEntity entity);
    // 批量更新
    void Update(IEnumerable<TEntity> entities);
    #endregion Update

    #region Delete
    // 删除
    void Delete(TEntity entity);
    // 根据主键(支持复合主键)删除
    void Delete(params object[] id);
    // 根据表达式条件批量删除
    void Delete(Expression<Func<TEntity, bool>> predicate);
    #endregion

    #region Aggregate
    bool Any();
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
    bool Any(Expression<Func<TEntity, bool>> predicate);
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    int Count();
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    int Count(Expression<Func<TEntity, bool>> predicate);
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    #endregion
```

**2. IUnitOfWork 工作单元接口**
```cs
    // 获取 DbContext
    public DbContext DbContext { get; }
    IDbConnection GetConnection();
    // 工作单元 提交
    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    // Dapper 封装
    Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string sql, object param = null, IDbContextTransaction trans = null) where TEntity : class;
    Task<int> ExecuteAsync(string sql, object param, IDbContextTransaction trans = null);
    // 开启事务
    IDbContextTransaction BeginTransaction();
    public IDbContextTransaction BeginTransaction(IsolationLevel isolationLevel);
    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    public Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken = default);
```

**3. Dapper事务**
```cs
public async Task InsertWithTransaction(User user1, User user2)
{
    using var tran = _unitOfWork.BeginTransaction();
    try
    {
        await _unitOfWork.ExecuteAsync("INSERT INTO User (Id,Name) VALUES ('1','张三'", user1, tran);
        await _unitOfWork.ExecuteAsync("INSERT INTO User (Id,Name) VALUES ('2','李四'", user2, tran);
        // 提交事务
        tran.Commit();
    }
    catch (Exception e)
    {
        // 异常回归事务
        tran.Rollback();
    }
}
```
**4. EF+Dapper混合事务**
```cs
public async Task InsertWithTransaction(User user1, User user2)
{
    using var tran = _unitOfWork.BeginTransaction();
    try
    {
        await _userRepository.InsertAsync(user1);
        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.ExecuteAsync("INSERT INTO User (Id,Name) VALUES ('2','李四'", user2, tran);
        // 提交事务
        tran.Commit();
    }
    catch (Exception e)
    {
        // 异常回归事务
        tran.Rollback();
    }
}
```
**5.其他**
- 支持多数据库连接，多个DbContext,具体请查看Demo

