using Microsoft.EntityFrameworkCore;
using RouletteTechTest.API.Data.Context;
using RouletteTechTest.API.Models.Entities;
using RouletteTechTest.API.Services.Interfaces;

namespace RouletteTechTest.API.Data.Repositories
{
    public class RoundRepository : IRoundRepository
    {
        private readonly ApplicationDbContext _context;

        public RoundRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Round?> GetActiveRoundAsync(Guid sessionId)
        {
            return await _context.Rounds
                .FirstOrDefaultAsync(r =>
                    r.SessionId == sessionId &&
                    !r.EndTime.HasValue);
        }

        public async Task AddRoundAsync(Round round)
        {
            await _context.Rounds.AddAsync(round);
        }

        public async Task UpdateAsync(Round round)
        {
            _context.Entry(round).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Round>> GetAllRoundsAsync()
        {
            return await _context.Rounds
                .Include(r => r.Bets)
                    .ThenInclude(b => b.User)
                .Include(r => r.Session)
                .Include(r => r.Result)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Round?> GetByIdAsync(Guid id)
        {
            return await _context.Rounds
                .Include(r => r.Bets)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task DeleteAsync(Guid id)
        {
            var round = await GetByIdAsync(id);
            if (round != null)
            {
                _context.Rounds.Remove(round);
            }
        }
        public async Task<IEnumerable<Round>> GetRoundsBySessionIdAsync(Guid sessionId)
        {
            return await _context.Rounds
                .Where(r => r.SessionId == sessionId)
                .Include(r => r.Bets)
                .Include(r => r.Result)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
