using Microsoft.EntityFrameworkCore;
using RouletteTechTest.API.Data.Context;
using RouletteTechTest.API.Models.Entities;
using RouletteTechTest.API.Services.Interfaces;

namespace RouletteTechTest.API.Data.Repositories
{
    public class SessionRepository : ISessionRepository
    {
        private readonly ApplicationDbContext _context;

        public SessionRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Session>> GetAllAsync()
        {
            return await _context.Sessions
                .Include(s => s.Players)
                .Include(s => s.Rounds)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Session?> GetByIdAsync(Guid id)
        {
            return await _context.Sessions
                .Include(s => s.Players)
                .Include(s => s.Rounds)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Session?> GetActiveSessionByUserAsync(string userName)
        {
            return await _context.Sessions
                .Include(s => s.Players) // 👈 Aseguramos que cargamos los usuarios
                .Where(s => s.EndTime == null && s.Players.Any(p => p.UserName == userName))
                .OrderByDescending(s => s.StartTime)
                .FirstOrDefaultAsync();
        }

        public async Task AddAsync(Session session)
        {
            await _context.Sessions.AddAsync(session);
        }

        public void Update(Session session)
        {
            _context.Entry(session).State = EntityState.Modified;
        }

        public async Task DeleteAsync(Guid id)
        {
            var session = await GetByIdAsync(id);
            if (session != null)
            {
                _context.Sessions.Remove(session);
            }
        }

        public async Task<bool> HasActiveSessionAsync(Guid userId)
        {
            return await _context.Sessions
                .AnyAsync(s => s.Players.Any(p => p.Id == userId) && s.EndTime == null);
        }

        public async Task AddPlayersToSession(Guid sessionId, IEnumerable<User> players)
        {
            var session = await GetByIdAsync(sessionId)
                ?? throw new KeyNotFoundException("Sesión no encontrada");

            session.Players.AddRange(players);
        }
    }
}

