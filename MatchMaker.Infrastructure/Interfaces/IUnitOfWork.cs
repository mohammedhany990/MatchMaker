namespace MatchMaker.Infrastructure.Interfaces
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IGenericRepository<T, TI> Repository<T, TI>() where T : class;

        Task<int> SaveAsync();

        //Task CommitTransactionAsync();
        //Task BeginTransactionAsync();
        //Task RollbackTransactionAsync();
    }
}
