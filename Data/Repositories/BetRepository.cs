using Microsoft.EntityFrameworkCore;
using RouletteTechTest.API.Data.Context;
using RouletteTechTest.API.Models.Entities;
using RouletteTechTest.API.Services.Interfaces;

namespace RouletteTechTest.API.Data.Repositories
{
    public class BetRepository : IBetRepository
    {
        private readonly ApplicationDbContext _context;

        public BetRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddBetAsync(Bet bet)
        {
            await _context.Bets.AddAsync(bet);
        }

        public async Task<List<Bet>> GetBetsByRoundAsync(Guid roundId)
        {
            return await _context.Bets
                .Where(b => b.RoundId == roundId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Bet>> GetAllAsync()
        {
            return await _context.Bets
                .ToListAsync ();
        }

        public async Task<Bet?> GetBetByIdAsync(Guid betId)
        {
            return await _context.Bets
                .Include(b => b.User.UserName)
                .Include(b => b.RoundId)
                .FirstOrDefaultAsync(b => b.Id == betId);
        }
        public Task<IEnumerable<Bet>> GetBetsByRoundIdAsync(Guid roundId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Bet>> GetBetsByUserIdAsync(Guid userId)
        {
            throw new NotImplementedException();
        }
        public Task UpdateBetAsync(Bet bet)
        {
            _context.Bets.Update(bet);
            return Task.CompletedTask;
        }

    }
}
