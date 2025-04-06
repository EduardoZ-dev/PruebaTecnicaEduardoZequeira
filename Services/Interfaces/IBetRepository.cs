using RouletteTechTest.API.Models.Entities;

namespace RouletteTechTest.API.Services.Interfaces
{
    public interface IBetRepository
    {
        Task AddBetAsync(Bet bet);
        Task<IEnumerable<Bet>> GetBetsByRoundIdAsync(Guid roundId);
        Task<IEnumerable<Bet>> GetBetsByUserIdAsync(Guid userId);
        Task<Bet?> GetBetByIdAsync(Guid betId);
        Task<IEnumerable<Bet>> GetAllAsync();
        Task<List<Bet>> GetBetsByRoundAsync(Guid roundId);
        Task UpdateBetAsync(Bet bet);
    }
}
