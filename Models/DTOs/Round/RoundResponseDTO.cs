using RouletteTechTest.API.Models.DTOs.Bet;
using RouletteTechTest.API.Models.DTOs.Common;

namespace RouletteTechTest.API.Models.DTOs.Round
{
    public class RoundResponseDTO
    {
        public Guid Id { get; set; }
        public int RoundNumber { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public Guid SessionId { get; set; }

        public SpinResultDTO? Result { get; set; }
        public List<BetDTO> Bets { get; set; } = new();
    }
}
