using RouletteTechTest.API.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace RouletteTechTest.API.Models.DTOs.Bet
{
    public class BetRequestDTO
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 50 caracteres")]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "El ID de la ronda es obligatorio")]
        public Guid RoundId { get; set; }

        [Required(ErrorMessage = "La cantidad apostada es obligatoria")]
        [Range(1, 10000, ErrorMessage = "El monto debe estar entre 1 y 10,000")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "El tipo de apuesta es obligatorio")]
        [EnumDataType(typeof(BetType), ErrorMessage = "Tipo de apuesta inválido")]
        public BetType Type { get; set; }

        [Required(ErrorMessage = "El valor de la apuesta es obligatorio")]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "El valor debe tener entre 1 y 10 caracteres")]
        public string BetValue { get; set; } = null!;
    }
}