using RouletteTechTest.API.Models.Entities;

namespace RouletteTechTest.API.Services.Interfaces
{
    public interface IRoundRepository
    {
        Task<IEnumerable<Round>> GetAllRoundsAsync();
        Task<Round?> GetByIdAsync(Guid id);
        Task<Round?> GetActiveRoundAsync(Guid sessionId);
        Task AddRoundAsync(Round round);
        Task UpdateAsync(Round round);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<Round>> GetRoundsBySessionIdAsync(Guid sessionId);
    }
}
