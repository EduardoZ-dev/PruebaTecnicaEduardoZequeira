using RouletteTechTest.API.Models.Entities;
using RouletteTechTest.API.Services.Interfaces;
using System;

namespace RouletteTechTest.API.Data.Repositories
{
    public class BetRepository : IBetRepository
    {
        private readonly AppDbContext _context;

        public BetRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Bet bet)
        {
            await _context.Bets.AddAsync(bet);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Bet>> GetBySessionIdAsync(Guid sessionId)
        {
            return await _context.Bets
                .Where(b => b.SessionId == sessionId)
                .ToListAsync();
        }

        // Método adicional útil: Obtener apuestas por usuario
        public async Task<List<Bet>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Bets
                .Include(b => b.Session)
                .Where(b => b.Session.UserId == userId)
                .ToListAsync();
        }
    }
}
