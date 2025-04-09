using RouletteTechTest.API.Models.DTOs.Bet;

namespace RouletteTechTest.API.Models.DTOs.Round
{
    public class RoundCloseResultDTO
    {
        public int ResultNumber { get; set; }
        public string Color { get; set; }
        public string Parity { get; set; }
        public List<BetDTO> Bets { get; set; } = new();
    }
}
