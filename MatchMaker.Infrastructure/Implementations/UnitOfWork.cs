using MatchMaker.Infrastructure.Identity;
using MatchMaker.Infrastructure.Interfaces;
using System.Collections;

namespace MatchMaker.Infrastructure.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppIdentityDbContext _dbContext;
        private Hashtable repositories = new Hashtable();
        public UnitOfWork(AppIdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public IGenericRepository<T, TI> Repository<T, TI>() where T : class
        {
            var key = typeof(T).Name;

            if (!repositories.ContainsKey(key))
            {
                var repository = new GenericRepository<T, TI>(_dbContext);
                repositories.Add(key, repository);
            }

            return (IGenericRepository<T, TI>)repositories[key];
        }


        public async Task<int> SaveAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await _dbContext.DisposeAsync();
        }
    }
}
