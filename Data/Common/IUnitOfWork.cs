using Microsoft.EntityFrameworkCore.Storage;
using RouletteTechTest.API.Services.Interfaces;

namespace RouletteTechTest.API.Data.Common
{
    public interface IUnitOfWork
    {
        ISessionRepository Sessions { get; }
        IUserRepository Users { get; }
        IRoundRepository Rounds { get; }
        IBetRepository Bets { get; }
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
        Task SaveChangesAsync();
    }
}
