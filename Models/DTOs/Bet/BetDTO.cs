namespace RouletteTechTest.API.Models.DTOs.Bet
{
    public class BetDTO
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = null!;
        public string BetType { get; set; } = null!;
        public string BetValue { get; set; } = null!;
        public decimal Amount { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Outcome { get; set; } = null!;
        public decimal Prize { get; set; }
    }
}
