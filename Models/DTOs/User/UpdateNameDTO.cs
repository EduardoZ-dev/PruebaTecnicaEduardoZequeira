using System.ComponentModel.DataAnnotations;

namespace RouletteTechTest.API.Models.DTOs.User
{
    public class UpdateNameDTO
    {
        [Required]
        public string NewUserName { get; set; } = null!;
    }
}
