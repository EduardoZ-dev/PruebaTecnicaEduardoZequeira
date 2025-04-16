using RouletteTechTest.API.Models;
using RouletteTechTest.API.Models.Entities;

namespace RouletteTechTest.API.Services
{
    public interface IRouletteService
    {
        SpinResult Spin();
        decimal CalculatePrize(BetRequest bet, SpinResult spinResult);
        Task<BetResponse> ProcessBet(BetRequest betRequest);
        Task UpdateUserBalanceAsync(string userName, decimal amount);
    }
}
