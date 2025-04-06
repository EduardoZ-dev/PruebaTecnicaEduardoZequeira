using RouletteTechTest.API.Models.DTOs.Round;
using RouletteTechTest.API.Models.Entities;

namespace RouletteTechTest.API.Services.Interfaces
{
    public interface IRoundService
    {
        Task<IEnumerable<RoundResponseDTO>> GetAllRoundAsync();
        Task<Round> StartRoundAsync(Guid sessionId);
        Task CloseRoundAsync(Guid roundId);
        Task<Round> GetRoundDetailsAsync(Guid roundId);
        Task<IEnumerable<Round>> GetRoundsBySessionAsync(Guid sessionId);

    }
}