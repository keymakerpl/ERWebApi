using ERService.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ERWebApi.SQLDataAccess.Repositories
{
    public class GenericRepository<TEntity, TContext> : IGenericRepository<TEntity>
        where TContext : DbContext
        where TEntity : class
    {
        //private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        protected readonly TContext Context;

        protected GenericRepository(TContext context)
        {
            Context = context;

            //Context.Database.Log = _logger.Debug;
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await Context.Set<TEntity>().ToListAsync();
        }

        public virtual TEntity GetById(Guid id)
        {
            return Context.Set<TEntity>().Find(id);
        }

        public virtual async Task<TEntity> GetByIdAsync(Guid id)
        {
            return await Context.Set<TEntity>().FindAsync(id);
        }

        public virtual async Task<IEnumerable<TEntity>> FindByAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Context.Set<TEntity>().Where(predicate).ToListAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> FindByIncludeAsync(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includeProps)
        {
            var query = GetAllIncluding(includeProps);            
            return await query.AsExpandable().Where(predicate).ToListAsync();
        }

        private IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] includeProps)
        {
            IQueryable<TEntity> queryable = Context.Set<TEntity>();
            return includeProps.Aggregate(queryable, (current, includeProperty) => current.Include(includeProperty));
        }

        public virtual async Task<IEnumerable<TEntity>> GetEntitiesAsync(int page = 1, int perPage = 10)
        {
            return await Context.Set<TEntity>().Skip((page - 1) * perPage).Take(perPage).ToListAsync();
        }

        public virtual List<T> Get<T>(string sqlQuery, object[] parameters)
        {
            //var result = Context.Database.SqlQuery<T>(sqlQuery, parameters).ToList();
            //return result;
            throw new NotImplementedException();
        }

        public virtual async Task<List<T>> GetAsync<T>(string sqlQuery, object[] parameters) 
        {
            //var result = await Context.Database.SqlQuery<T>(sqlQuery, parameters).ToListAsync();
            //return result;
            throw new NotImplementedException();
        }                

        public virtual async Task<bool> SaveAsync()
        {
            var entries = Context.ChangeTracker
                                .Entries<TEntity>()
                                .Where(x => (x.State == EntityState.Added || x.State == EntityState.Modified) &&
                                x.Entity != null && typeof(TEntity).IsAssignableFrom(x.Entity.GetType()))                                
                                .ToList();

            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        var modified = entry.Entity as IModificationHistory;
                        if (modified != null)
                            modified.DateModified = DateTime.Now;

                        var versioned = entry.Entity as IVersionedRow;
                        if (versioned != null)
                            versioned.RowVersion++;
                        break;

                    case EntityState.Added:
                        var added = entry.Entity as IModificationHistory;
                        if (added != null && added.DateAdded == DateTime.MinValue)
                            added.DateAdded = DateTime.Now;                        
                        break;                    
                }
            }

            var savedElements = await Context.SaveChangesAsync();
            return savedElements > 0;
        }

        public virtual bool HasChanges()
        {
            return Context.ChangeTracker.HasChanges();
        }

        public virtual void Add(TEntity model)
        {
            Context.Set<TEntity>().Add(model);
        }

        public virtual void Remove(TEntity model)
        {
            try
            {
                Context.Set<TEntity>().Remove(model);
            }
            catch (Exception ex)
            {
                //_logger.Debug(ex);
                //_logger.Error(ex);
            }
        }

        public virtual void RollBackChanges()
        {
            var changedEntries = Context.ChangeTracker
                                    .Entries()
                                    .Where(e => e.State != EntityState.Unchanged).ToList();

            foreach (var entry in changedEntries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    case EntityState.Deleted:
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Modified:
                        entry.CurrentValues.SetValues(entry.OriginalValues);
                        entry.State = EntityState.Unchanged;
                        break;
                }
            }
        }

        public virtual void ReloadEntities()
        {
            throw new NotImplementedException();
        }

        public virtual async Task ReloadEntitiesAsync()
        {
            throw new NotImplementedException();
        }

        public virtual void ReloadEntity(TEntity entity)
        {
            Context.Entry(entity).Reload();            
        }

        public virtual async Task ReloadEntityAsync(TEntity entity)
        {
            await Context.Entry(entity).ReloadAsync();            
        }

        public virtual async Task<PropertyValues> GetDatabaseValuesAsync(object entity)
        {
            var result = await Context.Entry(entity).GetDatabaseValuesAsync();
            return result;
        }
    }
}