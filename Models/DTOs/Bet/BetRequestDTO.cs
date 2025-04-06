using RouletteTechTest.API.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace RouletteTechTest.API.Models.DTOs.Bet
{
    public class BetRequestDTO
    {
        [Required] 
        public string UserName { get; set; }
        [Required] 
        public Guid RoundId { get; set; }

        [Required][Range(1, 10000)] 
        public decimal Amount { get; set; }
        [Required] 
        public BetType Type { get; set; } //0 Color 1Parity 2Number
        [Required] 
        public string BetValue { get; set; } = null!;
    }
}
