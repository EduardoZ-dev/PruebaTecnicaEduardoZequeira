using RouletteTechTest.API.Models.Entities;

namespace RouletteTechTest.API.Services.Interfaces
{
    public interface IBetRepository
    {
        Task AddAsync(Bet bet);
        Task<List<Bet>> GetBySessionIdAsync(Guid sessionId);
        Task<List<Bet>> GetByUserIdAsync(Guid userId); // ◄¡Nuevo método!
    }
}
