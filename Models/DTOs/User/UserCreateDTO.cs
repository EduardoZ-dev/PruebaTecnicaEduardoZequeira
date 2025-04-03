using System.ComponentModel.DataAnnotations;

namespace RouletteTechTest.API.Models.DTOs.User
{
    public class UserCreateDTO
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        [Range(10000000, 99999999)]
        public int DNI { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal InitialBalance { get; set; }
    }
}
