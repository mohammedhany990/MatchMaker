using MatchMaker.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MatchMaker.Infrastructure.Identity;

namespace MatchMaker.Infrastructure.Implementations
{
    public class GenericRepository<T, TI> : IGenericRepository<T, TI> where T : class
    {
        private readonly AppIdentityDbContext _dbContext;

        public GenericRepository(AppIdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<T?> GetAsync(TI id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        // Get with filter + includes
        public async Task<T?> GetAsync(
            Expression<Func<T, bool>> filter,
            Func<IQueryable<T>, IQueryable<T>>? include = null
            )  
        {
            IQueryable<T> query = _dbContext.Set<T>();
           
            if (include != null)
            {
                query = include(query);
            }

            return await query.FirstOrDefaultAsync(filter);
        }

        public async Task AddAsync(T item)
        {
            _dbContext.Set<T>().AddAsync(item);
        }

        public void Update(T item)
        {
            _dbContext.Set<T>().Update(item);
        }

        public void Delete(T item)
        {
            _dbContext.Set<T>().Remove(item);
        }

        public void DeleteRange(IEnumerable<T> items)
        {
            _dbContext.Set<T>().RemoveRange(items);
        }

        public Task<int> DeleteWhereAsync(Expression<Func<T, bool>> predicate)
        {
            return _dbContext.Set<T>()
                .Where(predicate)
                .ExecuteDeleteAsync();
        }

        public async Task<List<T>> GetAllAsync(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IQueryable<T>> include = null)
        {
            IQueryable<T> query = _dbContext.Set<T>();

            if (filter is not null)
            {
                query = query.Where(filter);
            }

            if (include is not null)
            {
                query = include(query);
            }

            return await query.ToListAsync();
        }

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>()
                .AsNoTracking()
                .FirstOrDefaultAsync(predicate);
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>()
                .AnyAsync(predicate);
        }
    }
}
