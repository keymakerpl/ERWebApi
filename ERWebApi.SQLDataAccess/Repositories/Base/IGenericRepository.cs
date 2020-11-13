using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ERWebApi.SQLDataAccess.Repositories
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        TEntity GetById(Guid id);
        Task<TEntity> GetByIdAsync(Guid id);
        Task<IEnumerable<TEntity>> FindByAsync(Expression<Func<TEntity, bool>> predicate);        
        Task<IEnumerable<TEntity>> GetAllAsync();
        List<T> Get<T>(string sqlQuery, object[] parameters);
        Task<List<T>> GetAsync<T>(string sqlQuery, object[] parameters);
        Task<IEnumerable<TEntity>> GetEntitiesAsync(int page = 1, int perPage = 10);
        Task<IEnumerable<TEntity>> FindByIncludeAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProp);        

        Task<bool> SaveAsync();

        bool HasChanges();
        void Add(TEntity model);
        void Remove(TEntity model);
        void RollBackChanges();
        /// <summary>
        /// Returns copy of db values of specific entity. Not tracked.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>Entity values from db</returns>
        Task<PropertyValues> GetDatabaseValuesAsync(object entity);
        /// <summary>
        /// Reload all tracked entieties with values from db
        /// </summary>
        /// <returns>Task</returns>
        Task ReloadEntitiesAsync();
        void ReloadEntity(TEntity entity);
        /// <summary>
        /// Reload specific entity with values from db
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>Task</returns>
        Task ReloadEntityAsync(TEntity entity);
    }
}