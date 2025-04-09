using RouletteTechTest.API.Models.Entities;

namespace RouletteTechTest.API.Services.Interfaces
{
    public interface ISessionRepository
    {
        Task<IEnumerable<Session>> GetAllAsync();
        Task<Session?> GetByIdAsync(Guid sessionId);
        Task<List<Session?>> GetByNameAsync(string userName);
        Task AddAsync(Session session);
        void Update(Session session);
        Task DeleteSessionsByUserAsync(string userName);
        Task<Session?> GetActiveSessionByUserAsync(string userName);
        Task AddPlayersToSession(Guid sessionId, IEnumerable<string> userNames);
    }
}
