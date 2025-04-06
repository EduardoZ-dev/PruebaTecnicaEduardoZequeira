using RouletteTechTest.API.Models.Entities;
using RouletteTechTest.API.Models.Enums;

namespace RouletteTechTest.API.Services.Interfaces
{
    public interface IBetCalculator
    {
        (BetOutcome Outcome, decimal Prize) CalculateResult(Bet bet, int winningNumber);
    }
}
