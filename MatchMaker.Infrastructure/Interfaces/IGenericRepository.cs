using MatchMaker.Core.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MatchMaker.Infrastructure.Interfaces
{
    public interface IGenericRepository<T, TI> where T : class
    {
        // Basic CRUD Operations
        Task<T?> GetAsync(TI id);

        public Task<T?> GetAsync(
            Expression<Func<T, bool>> filter,
            Func<IQueryable<T>, IQueryable<T>>? include = null);

        Task AddAsync(T item);
        void Update(T item);
        void Delete(T item);
        void DeleteRange(IEnumerable<T> items);
        Task<int> DeleteWhereAsync(Expression<Func<T, bool>> predicate);

        // Querying
        public Task<List<T>> GetAllAsync(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IQueryable<T>> include = null);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);


        Task<T> GetBySpecAsync(ISpecification<T> spec);
        Task<List<T>> GetAllWithSpecAsync(ISpecification<T> spec);

    }
}
