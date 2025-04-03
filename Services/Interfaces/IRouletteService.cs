using RouletteTechTest.API.Models.DTOs;
using RouletteTechTest.API.Models.DTOs.Bet;

namespace RouletteTechTest.API.Services.Interfaces
{
    public interface IRouletteService
    {
        Task<SpinResultDTO> SpinAsync();
        Task<BetResponseDTO> PlaceBetAsync(Guid userId, BetPlaceDTO bet);
    }
}
