using RouletteTechTest.API.Models.DTOs.Bet;
using RouletteTechTest.API.Models.Entities;

public interface IBetService
{
    Task<BetResultDTO> ProcessBetAndAdjustBalanceAsync(BetRequestDTO request);
    Task<List<Bet>> GetBetsByRoundAsync(Guid roundId);
    Task<Bet> GetBetByIdAsync(Guid betId);
    Task UpdateBetAsync(Bet bet);
}