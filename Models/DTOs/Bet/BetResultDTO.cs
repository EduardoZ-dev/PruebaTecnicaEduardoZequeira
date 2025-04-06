namespace RouletteTechTest.API.Models.DTOs.Bet
{
    public class BetResultDTO
    {
        public int? WinningNumber { get; set; }
        public string? Outcome { get; set; } = string.Empty;
        public decimal? Prize { get; set; }
        public decimal Balance { get; set; }
        public decimal BetAmount { get; set; }
        public string BetType { get; set; } = string.Empty;
        public string BetValue { get; set; } = string.Empty;
    }
}
