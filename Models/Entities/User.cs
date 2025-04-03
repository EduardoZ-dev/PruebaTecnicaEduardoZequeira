using System.ComponentModel.DataAnnotations;

namespace RouletteTechTest.API.Models.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres")]
        public string Name { get; set; } = null!;

        [Required]
        [Range(10000000, 99999999, ErrorMessage = "DNI inválido (8 dígitos)")]
        public int DNI { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "El saldo no puede ser negativo")]
        public decimal Balance { get; set; }

        // Navigation property
        public List<Session> Sessions { get; set; } = new();
    }
}
