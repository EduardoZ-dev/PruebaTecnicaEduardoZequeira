using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using RouletteTechTest.API.Models.DTOs.Common;
using System.Text.Json.Serialization;

namespace RouletteTechTest.API.Models.Entities
{
    public class Round
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string UserName { get; set; } = null!;

        [Required]
        public int RoundNumber { get; set; }

        [Required]
        public DateTime StartTime { get; set; } = DateTime.UtcNow;

        public DateTime? EndTime { get; set; }  // Null hasta que se gira la ruleta

        //Una ronda tiene muchas apuestas
        public List<Bet> Bets { get; set; } = new();

        public SpinResultDTO? Result { get; set; }  // Resultado del giro

        //Relacion con Session: Cada ronda pertenece a una sesion
        [Required]
        public Guid SessionId { get; set; }

        [ForeignKey("SessionId")]
        [JsonIgnore]
        public Session Session { get; set; }
    }
}
