using System.ComponentModel.DataAnnotations;

namespace RouletteTechTest.API.Models.DTOs.Session
{
    public class SessionCreateDTO
    {
        [Required(ErrorMessage = "Debe incluir al menos un jugador")]
        [MinLength(1)]
        public List<string> PlayerNames { get; set; } = new();
    }
}
