using RouletteTechTest.API.Models.Entities;
using RouletteTechTest.API.Services.Interfaces;
using System;

namespace RouletteTechTest.API.Data.Repositories
{
    public class SessionRepository : ISessionRepository
    {
        private readonly AppDbContext _context;

        public SessionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Session> GetByIdAsync(Guid id)
            => await _context.Sessions
                .Include(s => s.Bets)
                .FirstOrDefaultAsync(s => s.Id == id);

        public async Task AddAsync(Session session)
        {
            await _context.Sessions.AddAsync(session);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Session session)
        {
            _context.Sessions.Update(session);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Session>> GetByUserIdAsync(Guid userId)
            => await _context.Sessions
                .Where(s => s.UserId == userId)
                .ToListAsync();
    }
}
