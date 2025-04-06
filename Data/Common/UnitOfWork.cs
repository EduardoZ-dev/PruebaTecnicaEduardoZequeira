using Microsoft.EntityFrameworkCore.Storage;
using RouletteTechTest.API.Data.Context;
using RouletteTechTest.API.Services.Interfaces;


namespace RouletteTechTest.API.Data.Common
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(
            ApplicationDbContext context,
            ISessionRepository sessionRepository,
            IUserRepository userRepository,
            IRoundRepository roundRepository,
            IBetRepository betRepository)
        {
            _context = context;
            Sessions = sessionRepository;
            Users = userRepository;
            Rounds = roundRepository;
            Bets = betRepository;

        }

        public ISessionRepository Sessions { get; }
        public IUserRepository Users { get; }
        public IRoundRepository Rounds { get; }
        public IBetRepository Bets { get; }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
            return _transaction;
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
            await _transaction!.CommitAsync();
        }

        public async Task RollbackAsync()
            => await _transaction!.RollbackAsync();

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
