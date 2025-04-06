using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using RouletteTechTest.API.Models.Enums;

namespace RouletteTechTest.API.Models.Entities
{
    public class Bet
    {
        [Key]
        public Guid Id { get; set; }

        // Monto apostado por el usuario
        [Required]
        [Range(1, 10000)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public BetType Type { get; set; } //1 Color   2par 3numero

        [Required]
        public string BetValue { get; set; } = null!;

        // Momento en el que se realizó la apuesta
        [Required]
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;

        [Required]
        public BetOutcome Outcome { get; set; } = BetOutcome.Pending;


        [Column(TypeName = "decimal(18,2)")]
        public decimal Prize { get; set; }


        [Required]
        public Guid RoundId { get; set; }

        [ForeignKey("RoundId")]
        public Round Round { get; set; } = null!;

        //FK para con User
        [Required]
        public Guid UserId { get; set; } 

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
    }
}
