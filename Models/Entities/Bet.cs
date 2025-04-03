using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using RouletteTechTest.API.Models.Enums;

namespace RouletteTechTest.API.Models.Entities
{
    public class Bet
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid SessionId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public BetType Type { get; set; }

        public string? BetColor { get; set; }
        public int? BetNumber { get; set; }
        public string? BetParity { get; set; }


        [Required]
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;

        [Required]
        public BetOutcome Outcome { get; set; }

        public decimal Prize { get; set; }

        // Navigation properties
        [ForeignKey("SessionId")]
        public Session Session { get; set; }
    }
}
