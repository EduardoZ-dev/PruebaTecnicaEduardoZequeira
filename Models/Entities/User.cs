using System.ComponentModel.DataAnnotations;

namespace RouletteTechTest.API.Models.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string UserName { get; set; } = null!;
        [Required]
        public decimal Balance { get; set; }

    }
}
