using RouletteTechTest.API.Models.Entities;

namespace RouletteTechTest.API.Services.Interfaces
{
    public interface ISessionRepository
    {
        Task<IEnumerable<Session>> GetAllAsync();
        Task<Session?> GetByIdAsync(Guid id);
        Task AddAsync(Session session);
        void Update(Session session);
        Task DeleteAsync(Guid id);
        Task<Session?> GetActiveSessionByUserAsync(string userName);
        Task AddPlayersToSession(Guid sessionId, IEnumerable<User> players);
    }
}
