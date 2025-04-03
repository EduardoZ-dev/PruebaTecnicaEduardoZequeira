using RouletteTechTest.API.Models.Entities;

namespace RouletteTechTest.API.Services.Interfaces
{
    public interface ISessionRepository
    {
        Task<Session> GetByIdAsync(Guid id);
        Task AddAsync(Session session);
        Task UpdateAsync(Session session);
        Task<List<Session>> GetByUserIdAsync(Guid userId);
    }
}
