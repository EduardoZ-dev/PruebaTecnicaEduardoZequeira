using RouletteTechTest.API.Models.Enums;

namespace RouletteTechTest.API.Models.DTOs.Bet
{
    public class BetResponseDTO
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public BetOutcome Outcome { get; set; }
        public decimal Prize { get; set; }
    }

}

