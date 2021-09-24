using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NTier.Core.Repository
{
    public class EFRepository<T> : IRepository<T> where T : class, new()
    {
        #region Fields

        private readonly DbContext _context;

        private DbSet<T> _entities;

        #endregion

        #region Ctor

        public EFRepository(DbContext context)
        {
            this._context = context;
        }

        #endregion

        #region Properties

        public virtual IQueryable<T> Table
        {
            get
            {
                return Entities;
            }
        }

        public virtual IQueryable<T> TableNoTracking
        {
            get
            {
                return Entities.AsNoTracking();
            }
        }

        protected virtual DbSet<T> Entities
        {
            get
            {
                if (_entities == null)
                    _entities = _context.Set<T>();
                return _entities;
            }
        }

        #endregion

        #region Methods

        public async Task<T> AddIfNotExists(T entity, Expression<Func<T, bool>> match = null)
        {
            if (match != null)
            {
                var existingEntity = await Entities.AnyAsync(match);
                if (!existingEntity)
                {
                    var response = await Entities.AddAsync(entity);
                    return response.CurrentValues.ToObject() as T;
                }
            }
            return await Entities.Where(match).FirstOrDefaultAsync();
        }


        public T Add(T entity)
        {
            if (entity == null)
                return null;

            Entities.Add(entity);
            return entity;
        }

        public async Task<T> AddAsync(T entity)
        {
            if (entity == null)
                return null;

            await Entities.AddAsync(entity);
            return entity;
        }

        public List<T> AddRange(List<T> entities)
        {
            if (entities.Count <= 0)
                return null;

            Entities.AddRange(entities);
            return entities;
        }

        public async Task<List<T>> AddRangeAsync(List<T> entities)
        {
            if (entities.Count <= 0)
                return null;

            await Entities.AddRangeAsync(entities);
            return entities;
        }


        public T Update(T entity, Expression<Func<T, bool>> match)
        {
            var existingEntity = Entities.FirstOrDefault(match);

            if (existingEntity == null)
                return null;

            Entities.Update(entity);
            return existingEntity;
        }

        public async Task<T> UpdateAsync(T entity, Expression<Func<T, bool>> match)
        {
            var existingEntity = await Entities.FirstOrDefaultAsync(match);

            if (existingEntity == null)
                return null;

            Entities.Update(entity);

            return existingEntity;
        }


        public virtual T Update(T entity, params Expression<Func<T, object>>[] updatedProperties)
        {
            //dbEntityEntry.State = EntityState.Modified; --- I cannot do this.

            //Ensure only modified fields are updated.
            var dbEntityEntry = _context.Entry(entity);
            if (updatedProperties.Any())
            {
                //update explicitly mentioned properties
                foreach (var property in updatedProperties)
                {
                    dbEntityEntry.Property(property).IsModified = true;
                }
            }
            else
            {
                //no items mentioned, so find out the updated entries
                foreach (var property in dbEntityEntry.OriginalValues.Properties)
                {
                    var original = dbEntityEntry.OriginalValues.GetValue<object>(property);
                    var current = dbEntityEntry.CurrentValues.GetValue<object>(property);
                    if (original != null && !original.Equals(current))
                        dbEntityEntry.Property(property.Name).IsModified = true;
                }
            }
            return dbEntityEntry.Entity;
        }


        public void Delete(T entity)
        {
            if (entity == null)
                return;

            //if (entity is IAudityEntity)
            //{
            //    var baseEntity = entity as IAudityEntity;
            //    baseEntity.IsDeleted = true;
            //    baseEntity.IsActive = false;
            //    baseEntity.ModifiedDate = DateTime.Now;

            //    Entities.Update(entity);
            //}
            //else
            //{
            Entities.Remove(entity);
            //}
        }

        public T Get(Expression<Func<T, bool>> match)
        {
            return Entities.FirstOrDefault(match);
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> match)
        {
            return await Entities.FirstOrDefaultAsync(match);
        }

        public IQueryable<T> GetAllIncluding(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> queryable = Table;
            foreach (Expression<Func<T, object>> includeProperty in includeProperties)
            {
                queryable = queryable.Include(includeProperty);
            }

            return queryable;
        }

        public List<T> GetAll(Expression<Func<T, bool>> match)
        {
            return Table.Where(match).ToList();
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> match)
        {
            return await Entities.Where(match).ToListAsync();
        }

        public async Task<List<T>> GetWithPaging(int pageNumber, int pageSize, Expression<Func<T, bool>> match = null)
        {
            IQueryable<T> queryable = Table;
            int skipCount = pageNumber * pageSize;
            if (match != null)
            {
                queryable = queryable.Where(match);
            }
            var result = queryable.Skip(skipCount).Take(pageSize);
            return await result.ToListAsync();
        }

        public int Count()
        {
            return Entities.Count();
        }

        public async Task<int> CountAsync()
        {
            return await Entities.CountAsync();
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                this.disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

       

     



        #endregion
    }
}
