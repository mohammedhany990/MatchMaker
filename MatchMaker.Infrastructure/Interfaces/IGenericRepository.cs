﻿using MatchMaker.Core.Specifications;
using System.Linq.Expressions;

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
        IQueryable<T> GetAll();
        public Task<List<T>> GetAllAsync(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IQueryable<T>> include = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null);

        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);


        Task<T> GetBySpecAsync(ISpecification<T> spec);
        Task<List<T>> GetAllWithSpecAsync(ISpecification<T> spec);

    }
}
