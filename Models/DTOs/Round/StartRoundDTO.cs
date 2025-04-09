using System.ComponentModel.DataAnnotations;

namespace RouletteTechTest.API.Models.DTOs.Round
{
    public class StartRoundDTO
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        [StringLength(50, MinimumLength = 3)]
        public string UserName { get; set; } = null!;
    }
}
