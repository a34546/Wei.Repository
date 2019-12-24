using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Wei.Repository
{
    public class Repository<TEntity> : Repository<TEntity, int>, IRepository<TEntity>
        where TEntity : class, IEntity
    {
        public Repository(UnitOfWorkDbContext dbDbContext) : base(dbDbContext)
        {
        }
    }

    public class Repository<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey>
       where TEntity : class, IEntity<TPrimaryKey>
    {
        private readonly DbContext _context;
        private DbSet<TEntity> _entities;
        public Repository(UnitOfWorkDbContext context)
        {
            _context = context;
        }

        #region Get
        public virtual TEntity GetById(TPrimaryKey id)
        {
            if (id == null) throw new ArgumentNullException("id");

            return Entities.Find(id);
        }
        public virtual List<TEntity> GetAllList()
        {
            return Query.Where(x => x.IsDelete == false).ToList();
        }
        public virtual List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate)
        {
            return Query.Where(x => x.IsDelete == false).Where(predicate).ToList();
        }
        public virtual ValueTask<TEntity> GetByIdAsync(TPrimaryKey id)
        {
            if (id == null) throw new ArgumentNullException("id");

            return Entities.FindAsync(id);
        }
        public virtual Task<List<TEntity>> GetAllListAsync()
        {
            return Query.Where(x => x.IsDelete == false).ToListAsync();
        }
        public virtual Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Query.Where(x => x.IsDelete == false).Where(predicate).ToListAsync();
        }
        #endregion

        #region Insert
        public virtual void Insert(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            Entities.Add(entity);
        }
        public virtual void Insert(IEnumerable<TEntity> entities)
        {
            if (!entities.Any()) throw new ArgumentNullException("entities");
            Entities.AddRange(entities);
        }
        public virtual Task InsertAsync(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            Entities.AddAsync(entity);
            return Task.CompletedTask;
        }
        public virtual Task InsertAsync(IEnumerable<TEntity> entities)
        {
            if (!entities.Any()) throw new ArgumentNullException("entities");
            return Entities.AddRangeAsync(entities);
        }
        #endregion

        #region Update
        public virtual void Update(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            Entities.Attach(entity);
            _context.Update(entity);
        }
        public virtual void Update(IEnumerable<TEntity> entities)
        {
            if (!entities.Any()) throw new ArgumentNullException("entities");
            if (entities.Any(x => x == null)) throw new ArgumentNullException("Entity cannot be empty");
            _context.UpdateRange(entities);

        }
        public virtual void Update(TEntity entity, params Expression<Func<TEntity, object>>[] properties)
        {
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                if (string.IsNullOrEmpty(propertyName))
                {
                    propertyName = GetPropertyName(property.Body.ToString());
                }
                _context.Entry(entity).Property(propertyName).IsModified = true;

            }
        }
        public virtual Task UpdateAsync(TEntity entity)
        {
            Update(entity);
            return Task.CompletedTask;
        }
        public virtual Task UpdateAsync(IEnumerable<TEntity> entities)
        {
            if (!entities.Any()) throw new ArgumentNullException("entities");
            _context.UpdateRange(entities);
            return Task.CompletedTask;
        }
        public virtual Task UpdateAsync(TEntity entity, params Expression<Func<TEntity, object>>[] properties)
        {
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                if (string.IsNullOrEmpty(propertyName))
                {
                    propertyName = GetPropertyName(property.Body.ToString());
                }
                _context.Entry(entity).Property(propertyName).IsModified = true;

            }
            return Task.CompletedTask;
        }
        string GetPropertyName(string str)
        {
            return str.Split(',')[0].Split('.')[1];
        }
        #endregion

        #region Delete/HardDelete
        public virtual void Delete(TPrimaryKey id)
        {
            if (id == null) throw new ArgumentNullException("id");
            var entity = Entities.Find(id);
            if (entity != null)
            {
                entity.IsDelete = true;
                entity.DeleteTime = DateTime.Now;
                Update(entity);
            }
        }
        public virtual void Delete(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            entity.IsDelete = true;
            entity.DeleteTime = DateTime.Now;
            Update(entity);
        }
        public virtual void Delete(IEnumerable<TEntity> entities)
        {
            if (!entities.Any()) throw new ArgumentNullException("entities");
            foreach (var entity in entities)
            {
                entity.IsDelete = true;
                entity.DeleteTime = DateTime.Now;
            }
            Update(entities);
        }
        public virtual void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate == null) throw new ArgumentNullException("predicate");
            var entities = Entities.Where(predicate).ToList();
            if (entities != null && entities.Any())
            {
                Delete(entities);
            }
        }
        public virtual Task DeleteAsync(TPrimaryKey id)
        {
            if (id == null) throw new ArgumentNullException("id");
            var entity = Entities.Find(id);
            if (entity == null) throw new ArgumentNullException("entity");
            entity.IsDelete = true;
            entity.DeleteTime = DateTime.Now;
            return UpdateAsync(entity);
        }
        public virtual Task DeleteAsync(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            entity.IsDelete = true;
            entity.DeleteTime = DateTime.Now;
            return UpdateAsync(entity);
        }
        public virtual Task DeleteAsync(IEnumerable<TEntity> entities)
        {
            if (!entities.Any()) throw new ArgumentNullException("entities");
            foreach (var entity in entities)
            {
                entity.IsDelete = true;
                entity.DeleteTime = DateTime.Now;
            }
            return UpdateAsync(entities);
        }
        public virtual Task DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate == null) throw new ArgumentNullException("predicate");

            var entities = Entities.Where(predicate).ToList();
            if (entities != null && entities.Any()) return DeleteAsync(entities);
            return Task.CompletedTask;
        }
        public virtual void HardDelete(TPrimaryKey id)
        {
            if (id == null) throw new ArgumentNullException("id");
            var entity = Entities.Find(id);
            if (entity != null) _context.Remove(entity);
        }
        public virtual void HardDelete(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            _context.Remove(entity);
        }
        public virtual void HardDelete(IEnumerable<TEntity> entities)
        {
            if (!entities.Any()) throw new ArgumentNullException("entities");
            _context.RemoveRange(entities);
        }
        public virtual void HardDelete(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate == null) throw new ArgumentNullException("predicate");
            _context.RemoveRange(Entities.Where(predicate));
        }
        public virtual Task HardDeleteAsync(TPrimaryKey id)
        {
            if (id == null) throw new ArgumentNullException("id");
            var entity = Entities.Find(id);
            if (entity != null) _context.Remove(entity);
            return Task.CompletedTask;
        }
        public virtual Task HardDeleteAsync(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            _context.Remove(entity);
            return Task.CompletedTask;
        }
        public virtual Task HardDeleteAsync(IEnumerable<TEntity> entities)
        {
            if (!entities.Any()) throw new ArgumentNullException("entities");
            _context.RemoveRange(entities);
            return Task.CompletedTask;
        }
        public virtual Task HardDeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate == null) throw new ArgumentNullException("predicate");
            _context.RemoveRange(Entities.Where(predicate));
            return Task.CompletedTask;
        }
        #endregion

        #region Properties
        public virtual IQueryable<TEntity> Query => Entities;

        public virtual IQueryable<TEntity> QueryNoTracking => Entities.AsNoTracking();

        protected virtual DbSet<TEntity> Entities => _entities ?? (_entities = _context.Set<TEntity>());

        #endregion
    }
}
