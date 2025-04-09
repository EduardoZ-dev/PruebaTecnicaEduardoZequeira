using RouletteTechTest.API.Models.DTOs.Round;
using RouletteTechTest.API.Models.Entities;

namespace RouletteTechTest.API.Services.Interfaces
{
    public interface IRoundService
    {
        Task<IEnumerable<RoundResponseDTO>> GetAllRoundAsync();
        //Task<Round> CreateRoundAsync(string userName);
        Task<RoundDTO> StartRoundAsync(string userName);
        Task<RoundCloseResultDTO> CloseRoundAsync(Guid roundId);
        Task<Round> GetRoundDetailsAsync(Guid roundId);
        Task<RoundDTO> GetCurrentActiveRoundAsync(string userName);


    }
}