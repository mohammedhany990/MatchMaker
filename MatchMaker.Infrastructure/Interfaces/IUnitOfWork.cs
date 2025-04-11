using MatchMaker.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchMaker.Infrastructure.Interfaces
{
   public interface IUnitOfWork: IAsyncDisposable
    {
        IGenericRepository<T,TI> Repository<T,TI>() where T : class;
        
        Task<int> SaveAsync();

        //Task CommitTransactionAsync();
        //Task BeginTransactionAsync();
        //Task RollbackTransactionAsync();
    }
}
