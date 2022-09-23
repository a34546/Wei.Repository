using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wei.Repository
{
    public abstract class RepositoryBase1<TEntity> : IRepositoryBase<TEntity>
        where TEntity : class
    {

        public abstract IQueryable<TEntity> Query();
        public abstract IQueryable<TEntity> QueryNoTracking();
        public virtual IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate) => Query().Where(predicate);
        public virtual IQueryable<TEntity> QueryNoTracking(Expression<Func<TEntity, bool>> predicate) => QueryNoTracking().Where(predicate);
        public virtual IEnumerable<TEntity> GetAll() => Query().ToList();
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default) => await Query().ToListAsync(cancellationToken);
        public virtual IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate) => Query().Where(predicate).ToList();
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) => await Query().Where(predicate).ToListAsync(cancellationToken);
        public abstract TEntity Get(object id);
        public abstract ValueTask<TEntity> GetAsync(object id, CancellationToken cancellationToken = default);
        public virtual TEntity FirstOrDefault() => Query().FirstOrDefault();
        public virtual Task<TEntity> FirstOrDefaultAsync(CancellationToken cancellationToken = default) => Query().FirstOrDefaultAsync(cancellationToken);
        public virtual TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate) => Query().FirstOrDefault(predicate);
        public virtual Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) => Query().FirstOrDefaultAsync(predicate, cancellationToken);
        public abstract TEntity Insert(TEntity entity);
        public abstract Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default);
        public abstract void Insert(IEnumerable<TEntity> entities);
        public abstract Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
        public abstract TEntity Update(TEntity entity);
        public abstract void Update(IEnumerable<TEntity> entities);
        public abstract void Delete(TEntity entity);
        public abstract void Delete(object id);
        public abstract void Delete(Expression<Func<TEntity, bool>> predicate);
        public bool Any() => Query().Any();
        public Task<bool> AnyAsync(CancellationToken cancellationToken = default) => Query().AnyAsync(cancellationToken);
        public bool Any(Expression<Func<TEntity, bool>> predicate) => Query().Any(predicate);
        public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) => Query().AnyAsync(predicate, cancellationToken);
        public virtual int Count() => Query().Count();
        public virtual Task<int> CountAsync(CancellationToken cancellationToken = default) => Query().CountAsync(cancellationToken);
        public virtual int Count(Expression<Func<TEntity, bool>> predicate) => Query().Where(predicate).Count();
        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) => Query().CountAsync(predicate, cancellationToken);
    }


    public abstract class RepositoryBase<TEntity> : IRepositoryBase<TEntity>
        where TEntity : class
    {
        internal abstract DbContext DbContext { get; set; }
        internal virtual DbSet<TEntity> Table => DbContext.Set<TEntity>();

        public IQueryable<TEntity> Query() => Table.AsQueryable();
        public IQueryable<TEntity> QueryNoTracking() => Table.AsQueryable().AsNoTracking();
        public virtual TEntity Get(object id) => Table.Find(id);
        public virtual ValueTask<TEntity> GetAsync(object id, CancellationToken cancellationToken = default) => Table.FindAsync(new object[] { id }, cancellationToken: cancellationToken);
        public virtual IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate) => Query().Where(predicate);
        public virtual IQueryable<TEntity> QueryNoTracking(Expression<Func<TEntity, bool>> predicate) => QueryNoTracking().Where(predicate);
        public virtual IEnumerable<TEntity> GetAll() => Query().ToList();
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default) => await Query().ToListAsync(cancellationToken);
        public virtual IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate) => Query().Where(predicate).ToList();
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) => await Query().Where(predicate).ToListAsync(cancellationToken);
        public virtual TEntity FirstOrDefault() => Query().FirstOrDefault();
        public virtual Task<TEntity> FirstOrDefaultAsync(CancellationToken cancellationToken = default) => Query().FirstOrDefaultAsync(cancellationToken);
        public virtual TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate) => Query().FirstOrDefault(predicate);
        public virtual Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) => Query().FirstOrDefaultAsync(predicate, cancellationToken);

        public virtual TEntity Insert(TEntity entity) => Table.Add(entity).Entity;
        public virtual async Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            var entityEntry = await Table.AddAsync(entity, cancellationToken);
            return entityEntry.Entity;
        }
        public virtual void Insert(IEnumerable<TEntity> entities) => Table.AddRange(entities);
        public virtual Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) => Table.AddRangeAsync(entities, cancellationToken);
        public virtual TEntity Update(TEntity entity)
        {
            DbContext.Attach(entity);
            DbContext.Update(entity);
            return entity;
        }
        public virtual void Update(IEnumerable<TEntity> entities) => DbContext.UpdateRange(entities);
        public virtual void Delete(TEntity entity) => Table.Remove(entity);
        public virtual void Delete(object id)
        {
            var entity = Get(id);
            if (entity != null)
                Delete(entity);
        }
        public virtual void Delete(Expression<Func<TEntity, bool>> predicate) => Table.RemoveRange(Table.Where(predicate));
        public bool Any() => Query().Any();
        public Task<bool> AnyAsync(CancellationToken cancellationToken = default) => Query().AnyAsync(cancellationToken);
        public bool Any(Expression<Func<TEntity, bool>> predicate) => Query().Any(predicate);
        public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) => Query().AnyAsync(predicate, cancellationToken);
        public virtual int Count() => Query().Count();
        public virtual Task<int> CountAsync(CancellationToken cancellationToken = default) => Query().CountAsync(cancellationToken);
        public virtual int Count(Expression<Func<TEntity, bool>> predicate) => Query().Where(predicate).Count();
        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) => Query().CountAsync(predicate, cancellationToken);
    }
}
