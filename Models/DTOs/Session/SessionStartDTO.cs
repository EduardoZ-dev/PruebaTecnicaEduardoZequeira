using System.ComponentModel.DataAnnotations;

namespace RouletteTechTest.API.Models.DTOs.Session
{
    public class SessionStartDTO
    {
        [Required]
        public Guid UserId { get; set; }
    }
}
