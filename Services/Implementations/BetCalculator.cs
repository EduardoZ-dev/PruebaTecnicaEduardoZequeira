using RouletteTechTest.API.Models.Entities;
using RouletteTechTest.API.Models.Enums;
using RouletteTechTest.API.Services.Interfaces;

namespace RouletteTechTest.API.Services.Implementations
{
    public class BetCalculator : IBetCalculator
    {
        public (BetOutcome Outcome, decimal Prize) CalculateResult(Bet bet, int winningNumber)
        {
            return bet.Type switch
            {
                BetType.Number => CalculateNumberBet(bet, winningNumber),
                BetType.Color => CalculateColorBet(bet, winningNumber),
                BetType.Parity => CalculateParityBet(bet, winningNumber),
                _ => (BetOutcome.Lose, 0)
            };
        }

        private (BetOutcome, decimal) CalculateNumberBet(Bet bet, int winningNumber)
        {
            var isWin = int.TryParse(bet.BetValue, out int betNumber) &&
                       betNumber == winningNumber;

            return (
                isWin ? BetOutcome.Win : BetOutcome.Lose,
                isWin ? bet.Amount * 35 : 0
            );
        }

        private (BetOutcome, decimal) CalculateColorBet(Bet bet, int winningNumber)
        {
            var actualColor = GetColor(winningNumber);
            return (
                bet.BetValue.Equals(actualColor, StringComparison.OrdinalIgnoreCase) ?
                    BetOutcome.Win : BetOutcome.Lose,
                bet.Amount * 2
            );
        }

        private (BetOutcome, decimal) CalculateParityBet(Bet bet, int winningNumber)
        {
            if (winningNumber == 0) return (BetOutcome.Lose, 0); // El 0 no es par ni impar

            var actualParity = winningNumber % 2 == 0 ? "even" : "odd";
            return (
                bet.BetValue.Equals(actualParity, StringComparison.OrdinalIgnoreCase) ?
                    BetOutcome.Win : BetOutcome.Lose,
                bet.Amount * 2
            );
        }

        private static string GetColor(int number)
        {
            if (number == 0) return "green";

            int[] redNumbers = {
                1, 3, 5, 7, 9, 12, 14, 16, 18,
                19, 21, 23, 25, 27, 30, 32, 34, 36
            };

            return redNumbers.Contains(number) ? "red" : "black";
        }
    }
}
