using RouletteTechTest.API.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace RouletteTechTest.API.Models.DTOs.Bet
{
    public class BetPlaceDTO
    {
        [Required]
        public BetType Type { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        public string Color { get; set; }
        public int? Number { get; set; }
        public string Parity { get; set; }
    }
}
