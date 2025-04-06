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

        //Relacion muchos a muchos, User => Sessions
        public List<Session> Sessions { get; set; } = new();
    }
}
