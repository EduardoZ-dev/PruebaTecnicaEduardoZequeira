using RouletteTechTest.API.Models.DTOs.Round;
using System.ComponentModel.DataAnnotations;

namespace RouletteTechTest.API.Models.DTOs.Session
{
    public class SessionCreateDTO
    {
        public Guid Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public List<RoundDTO> Rounds { get; set; }
        [Required(ErrorMessage = "Debe incluir al menos un jugador")]
        [MinLength(1)]
        public List<string> PlayerNames { get; set; } = new();
    }
}
