
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Services
{
    public class Service<TContext> : IService<TContext> where TContext : DbContext
    {
        protected readonly TContext _context;
        public Service(TContext context) => _context = context;

        /// <summary>
        /// Extension to allow generic key comparisons inside IQueryables
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="key"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <returns></returns>
        public Expression<Func<TEntity, bool>> KeyMatch<TEntity, TKey>(Expression<Func<TEntity, TKey>> expression, TKey key)
            where TEntity : Entity<TKey>
        {
            var memberExpression = (MemberExpression)expression.Body;
            var parameter = Expression.Parameter(typeof(TEntity), "x");
            var property = Expression.Property(parameter, memberExpression.Member.Name);
            var equal = Expression.Equal(property, Expression.Constant(key));
            return Expression.Lambda<Func<TEntity, bool>>(equal, parameter);
        }

        #region getters
        /// <summary>
        /// Get an entity from database entity set with given int key.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="key"></param>
        /// <param name="includes"></param>
        /// <param name="isreadonly"></param>
        /// <returns></returns>
        public TEntity Get<TEntity>(int key, string includes = "", bool isreadonly = false) where TEntity : Entity<int>
        {
            return Get<TEntity, int>(key, includes, isreadonly);
        }

        /// <summary>
        /// Get all entities with int keys from database entity set.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="isreadonly"></param>
        /// <returns></returns>
        public IQueryable<TEntity> GetAll<TEntity>(bool isreadonly = false) where TEntity : Entity<int>
        {
            return GetAll<TEntity, int>(isreadonly);
        }

        /// <summary>
        /// Get all entities with int keys from database entity set, including desired navigation properties.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="includes"></param>
        /// <param name="isreadonly"></param>
        /// <returns></returns>
        public IQueryable<TEntity> GetAll<TEntity>(string includes, bool isreadonly = false) where TEntity : Entity<int>
        {
            return GetAll<TEntity, int>(includes, isreadonly);
        }

        /// <summary>
        /// Retrieve entities with optional expression predicate, ordering and property includes.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="orderExpression"></param>
        /// <param name="skip"></param>
        /// <param name="top"></param>
        /// <param name="includes"></param>
        /// <param name="isreadonly"></param>
        /// <returns></returns>
        public IQueryable<TEntity> Find<TEntity>(
            Expression<Func<TEntity, bool>> predicate = null, 
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderExpression = null, 
            int? skip = null, 
            int? top = null,
            string includes = "", 
            bool isreadonly = false) where TEntity : Entity<int>
        {
            return Find<TEntity, int>(predicate, orderExpression, skip, top, includes, isreadonly);
        }

        /// <summary>
        /// Get an entity from database entity set with given key.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="key"></param>
        /// <param name="includes"></param>
        /// <param name="isreadonly"></param>
        /// <returns></returns>
        public TEntity Get<TEntity, TKey>(TKey key, string includes = "", bool isreadonly = false) where TEntity : Entity<TKey>
        {
            if (isreadonly)
                return Find<TEntity, TKey>(KeyMatch<TEntity, TKey>(e => e.Id, key), includes: includes).AsNoTracking().SingleOrDefault();

            return Find<TEntity, TKey>(KeyMatch<TEntity, TKey>(e => e.Id, key), includes: includes).SingleOrDefault();
        }

        /// <summary>
        /// Get all entities from database entity set.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="isreadonly"></param>
        /// <returns></returns>
        public IQueryable<TEntity> GetAll<TEntity, TKey>(bool isreadonly = false) where TEntity : Entity<TKey>
         => isreadonly ? _context.Set<TEntity>().AsNoTracking() : _context.Set<TEntity>();

        /// <summary>
        /// Get all entities from database entity set, including desired navigation properties.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="includes"></param>
        /// <param name="isreadonly"></param>
        /// <returns></returns>
        public IQueryable<TEntity> GetAll<TEntity, TKey>(string includes, bool isreadonly = false) where TEntity : Entity<TKey>
        => Find<TEntity, TKey>(includes: includes, isreadonly: isreadonly);

        /// <summary>
        /// Retrieve entities with optional expression predicate, ordering and property includes.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="orderExpression"></param>
        /// <param name="skip"></param>
        /// <param name="top"></param>
        /// <param name="includes"></param>
        /// <param name="isreadonly"></param>
        /// <returns></returns>
        public IQueryable<TEntity> Find<TEntity, TKey>(
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderExpression = null,
            int? skip = null,
            int? top = null,
            string includes = "",
            bool isreadonly = false) where TEntity : Entity<TKey>
        {
            IQueryable<TEntity> query = _context.Set<TEntity>().Where(predicate ?? (e => true));

            orderExpression = orderExpression ?? (q => q.OrderBy(e => e.Id));

            query = top != null ? orderExpression(query).Skip(skip ?? 0).Take(top.Value)
                                : orderExpression(query).Skip(skip ?? 0);

            foreach (var property in includes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                query = query.Include(property);

            return isreadonly ? query.AsNoTracking() : query;
        }
        #endregion

        #region setters
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public TEntity Add<TEntity>(TEntity entity) where TEntity : Entity<int> 
            => Add<TEntity, int>(entity);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public IEnumerable<TEntity> AddRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : Entity<int> 
            => AddRange<TEntity, int>(entities);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public TEntity Update<TEntity>(TEntity entity) where TEntity : Entity<int> 
            => Update<TEntity, int>(entity);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public IEnumerable<TEntity> UpdateRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : Entity<int>
            => UpdateRange<TEntity, int>(entities);

        /// <summary>
        /// Add given entity to database entity set.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public TEntity Add<TEntity, TKey>(TEntity entity) where TEntity : Entity<TKey> 
            => _context.Set<TEntity>().Add(entity);

        /// <summary>
        /// Add given entities to database entity set.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public IEnumerable<TEntity> AddRange<TEntity, TKey>(IEnumerable<TEntity> entities) where TEntity : Entity<TKey> 
            => _context.Set<TEntity>().AddRange(entities);

        /// <summary>
        /// Set given entity to be updated to database entity set.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public TEntity Update<TEntity, TKey>(TEntity entity) where TEntity : Entity<TKey>
        {
            _context.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        /// <summary>
        /// Set given entities to be updated to database entity set.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public IEnumerable<TEntity> UpdateRange<TEntity, TKey>(IEnumerable<TEntity> entities) where TEntity : Entity<TKey>
        => entities.Select(e => { _context.Entry(e).State = EntityState.Modified; return e; });

        #endregion

        #region removals
        /// <summary>
        /// Remove given entity from database entity set.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public TEntity Remove<TEntity, TKey>(TEntity entity) where TEntity : Entity<TKey> 
            => _context.Set<TEntity>().Remove(entity);

        /// <summary>
        /// Remove an entity from database entity set got by given key.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public TEntity Remove<TEntity, TKey>(TKey key) where TEntity : Entity<TKey> 
            => _context.Set<TEntity>().Remove(_context.Set<TEntity>().Find(key));

        /// <summary>
        /// Remove all given entities from database set.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public IEnumerable<TEntity> RemoveRange<TEntity, TKey>(IEnumerable<TEntity> entities) where TEntity : Entity<TKey> 
            => _context.Set<TEntity>().RemoveRange(entities);

        /// <summary>
        /// Remove all entities from database set with key in given keys.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="keys"></param>
        /// <returns></returns>
        public IEnumerable<TEntity> RemoveRange<TEntity, TKey>(IEnumerable<TKey> keys) where TEntity : Entity<TKey>
        {
            var entities = _context.Set<TEntity>()
                                   .Join(keys,
                                        entity => entity.Id,
                                        key => key,
                                        (entity, key) => entity);

            return _context.Set<TEntity>().RemoveRange(entities);
        }

        /// <summary>
        /// Remove given entity with int key from database entity set.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public TEntity Remove<TEntity>(TEntity entity) where TEntity : Entity<int> 
            => Remove<TEntity, int>(entity);

        /// <summary>
        /// Remove an entity with int key from database entity set got by given key.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public TEntity Remove<TEntity>(int key) where TEntity : Entity<int> 
            => Remove<TEntity, int>(key);

        /// <summary>
        /// Remove all given entities with int keys from database set.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public IEnumerable<TEntity> RemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : Entity<int> 
            => RemoveRange<TEntity, int>(entities);

        /// <summary>
        /// Remove all entities with int keys from database set with key in given keys.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="keys"></param>
        /// <returns></returns>
        public IEnumerable<TEntity> RemoveRange<TEntity>(IEnumerable<int> keys) where TEntity : Entity<int> 
            => RemoveRange<TEntity, int>(keys);
        #endregion

        #region finishers
        /// <summary>
        /// Commit changes on entities to database or fails if no related context instance found.
        /// </summary>
        public int Commit()
        {
            if (_context != null) return _context.SaveChanges();
            else throw new NullReferenceException($"Commit fails. No instance of related {_context.GetType().Name} context found.");
        }

        /// <summary>
        /// Rollback changes on entities to database to avoid missmanipulation of errores,
        /// or fails if no related context instance found.
        /// </summary>
        public void Rollback()
        {
            if (_context != null) _context.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
            else throw new NullReferenceException($"Commit fails. No instance of related {_context.GetType().Name} context found.");
        }
        #endregion
    }
}