using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RouletteTechTest.API.Models.Entities
{
    public class Session
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public DateTime StartTime { get; set; } = DateTime.UtcNow;

        public DateTime? EndTime { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal InitialBalance { get; set; }

        [Range(0, double.MaxValue)]
        public decimal FinalBalance { get; set; }

        // Foreign Key
        [Required]
        public Guid UserId { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public User User { get; set; }

        public List<Bet> Bets { get; set; } = new();

        // Propiedades calculadas (no se mapean a la DB)
        [NotMapped]
        public decimal TotalBets => Bets.Sum(b => b.Amount);

        [NotMapped]
        public decimal NetProfit => FinalBalance - InitialBalance;
    }
}
