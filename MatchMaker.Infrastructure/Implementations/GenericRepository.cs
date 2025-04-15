using MatchMaker.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using MatchMaker.Core.Specifications;
using MatchMaker.Infrastructure.Identity;

namespace MatchMaker.Infrastructure.Implementations
{
    public class GenericRepository<T, TId> : IGenericRepository<T, TId> where T : class
    {
        private readonly AppIdentityDbContext _dbContext;

        public GenericRepository(AppIdentityDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        #region CRUD Operations

        public async Task<T?> GetAsync(TId id) => await _dbContext.Set<T>().FindAsync(id);

        public async Task AddAsync(T entity) => await _dbContext.Set<T>().AddAsync(entity);

        public void Update(T entity) => _dbContext.Set<T>().Update(entity);

        public void Delete(T entity) => _dbContext.Set<T>().Remove(entity);

        public void DeleteRange(IEnumerable<T> entities) => _dbContext.Set<T>().RemoveRange(entities);

        public Task<int> DeleteWhereAsync(Expression<Func<T, bool>> predicate) =>
            _dbContext.Set<T>().Where(predicate).ExecuteDeleteAsync();

        #endregion



        #region Query Operations
       
        public async Task<T?> GetAsync(
            Expression<Func<T, bool>> filter,
            Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            var query = _dbContext.Set<T>().AsQueryable();
            if (include != null)
            {
                query = include(query);
            }
            return await query.FirstOrDefaultAsync(filter);
        }

        public async Task<List<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            var query = _dbContext.Set<T>().AsQueryable();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (include != null)
            {
                query = include(query);
            }

            return await query.ToListAsync();
        }

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate) =>
            await _dbContext.Set<T>()
                .AsNoTracking()
                .FirstOrDefaultAsync(predicate);

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate) =>
            await _dbContext.Set<T>().AnyAsync(predicate);

        #endregion



        #region Specification Pattern

        public async Task<List<T>> GetAllWithSpecAsync(ISpecification<T> spec) =>
            await ApplySpecification(spec).ToListAsync();

        public async Task<T?> GetBySpecAsync(ISpecification<T> spec) =>
            await ApplySpecification(spec).FirstOrDefaultAsync();

        private IQueryable<T> ApplySpecification(ISpecification<T> spec) =>
            SpecificationEvaluator<T>.GetQuery(_dbContext.Set<T>().AsQueryable(), spec);

        #endregion
    }
}