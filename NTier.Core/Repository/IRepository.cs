using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NTier.Core.Repository
{
    public interface IRepository<T> where T : class,new()
    {
        Task<T> AddIfNotExists(T entity, Expression<Func<T, bool>> match = null);
        T Get(Expression<Func<T, bool>> match);
        Task<T> GetAsync(Expression<Func<T, bool>> match);
        List<T> GetAll(Expression<Func<T, bool>> match);
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> match);
        IQueryable<T> GetAllIncluding(params Expression<Func<T, object>>[] includeProperties);
        Task<List<T>> GetWithPaging(int pageNumber, int pageSize, Expression<Func<T, bool>> match = null);

        T Add(T entity);
        Task<T> AddAsync(T entity);
        List<T> AddRange(List<T> entities);
        Task<List<T>> AddRangeAsync(List<T> entities);
        void Delete(T entity);
        T Update(T entity, Expression<Func<T, bool>> match);
        T Update(T entity, params Expression<Func<T, object>>[] updatedProperties);
        Task<T> UpdateAsync(T entity, Expression<Func<T, bool>> match);

        int Count();
        Task<int> CountAsync();

        IQueryable<T> Table { get; }
        IQueryable<T> TableNoTracking { get; }

        void Dispose();
    }
}
