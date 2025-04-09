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

        public async Task<Session?> GetByIdAsync(Guid sessionId)
        {
            return await _context.Sessions
                .Include(s => s.Players) // Incluir jugadores
                .Include(s => s.Rounds)   // Incluir rondas
                .FirstOrDefaultAsync(s => s.Id == sessionId);
        }

        public async Task<List<Session?>> GetByNameAsync(string userName)
        {
            return await _context.Sessions
                .Where(s => s.Players.Any(p => p.UserName == userName))
                .Include(s => s.Rounds)
                .OrderByDescending(s => s.StartTime)
                .ToListAsync();
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

        public async Task DeleteSessionsByUserAsync(string userName)
        {
            // Obtener todas las sesiones del usuario
            var sessions = await _context.Sessions
                .Where(s => s.Players.Any(p => p.UserName == userName))
                .ToListAsync();

            _context.Sessions.RemoveRange(sessions);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasActiveSessionAsync(Guid userId)
        {
            return await _context.Sessions
                .AnyAsync(s => s.Players.Any(p => p.Id == userId) && s.EndTime == null);
        }

        public async Task AddPlayersToSession(Guid sessionId, IEnumerable<string> userNames)
        {
            var session = await _context.Sessions
                .Include(s => s.Players)
                .FirstOrDefaultAsync(s => s.Id == sessionId)
                ?? throw new KeyNotFoundException("Sesión no encontrada");

            // Obtener usuarios existentes
            var existingUsers = await _context.Users
                .Where(u => userNames.Contains(u.UserName))
                .ToListAsync();

            // Validar usuarios no encontrados
            var notFound = userNames.Except(existingUsers.Select(u => u.UserName));
            if (notFound.Any())
            {
                throw new ArgumentException($"Usuarios no existen: {string.Join(", ", notFound)}");
            }

            // Añadir solo nuevos jugadores
            var newPlayers = existingUsers.Where(u => !session.Players.Contains(u));
            session.Players.AddRange(newPlayers);

            await _context.SaveChangesAsync();
        }
    }
}

