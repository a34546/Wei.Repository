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
    public abstract class RepositoryBase<TEntity> : IRepositoryBase<TEntity>
        where TEntity : class
    {
        internal abstract DbContext DbContext { get; set; }
        internal DbSet<TEntity> Table => DbContext.Set<TEntity>();

        public IQueryable<TEntity> Query() => Table.AsQueryable();
        public IQueryable<TEntity> QueryNoTracking() => Table.AsQueryable().AsNoTracking();
        public virtual TEntity Get(params object[] id) => Table.Find(id);
        public virtual ValueTask<TEntity> GetAsync(params object[] id) => Table.FindAsync(id);
        public virtual ValueTask<TEntity> GetAsync(object[] ids, CancellationToken cancellationToken = default) => Table.FindAsync(ids, cancellationToken: cancellationToken);
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
            AttachEntity(entity);
            DbContext.Update(entity);
            return entity;
        }
        public virtual void Update(IEnumerable<TEntity> entities) => DbContext.UpdateRange(entities);
        public virtual IEnumerable<TEntity> Update(Expression<Func<TEntity, bool>> predicate, Action<TEntity> updateAction)
        {
            var entities = Query(predicate).ToList();
            entities?.ForEach(updateAction);
            return entities;
        }
        public virtual async Task<IEnumerable<TEntity>> UpdateAsync(Expression<Func<TEntity, bool>> predicate, Action<TEntity> updateAction, CancellationToken cancellationToken = default)
        {
            var entities = await Query(predicate).ToListAsync(cancellationToken);
            entities?.ForEach(updateAction);
            return entities;
        }

        public virtual void Delete(TEntity entity) => Table.Remove(entity);
        public virtual void Delete(params object[] id)
        {
            var entity = Get(id);
            if (entity != null)
                Delete(entity);
        }
        public virtual void Delete(Expression<Func<TEntity, bool>> predicate) => Table.RemoveRange(Table.Where(predicate));
        public virtual bool Any() => Query().Any();
        public virtual Task<bool> AnyAsync(CancellationToken cancellationToken = default) => Query().AnyAsync(cancellationToken);
        public virtual bool Any(Expression<Func<TEntity, bool>> predicate) => Query().Any(predicate);
        public virtual Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) => Query().AnyAsync(predicate, cancellationToken);
        public virtual int Count() => Query().Count();
        public virtual Task<int> CountAsync(CancellationToken cancellationToken = default) => Query().CountAsync(cancellationToken);
        public virtual int Count(Expression<Func<TEntity, bool>> predicate) => Query().Where(predicate).Count();
        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) => Query().CountAsync(predicate, cancellationToken);

        private void AttachEntity(TEntity entity)
        {
            var d = DbContext.ChangeTracker.Entries().ToList();
            var entry = DbContext.ChangeTracker.Entries().FirstOrDefault(ent => ent.Entity == entity);
            if (entry == null) DbContext.Attach(entity);
        }
    }
}
