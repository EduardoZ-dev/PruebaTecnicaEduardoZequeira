using System.ComponentModel.DataAnnotations;

namespace RouletteTechTest.API.Models.Entities
{
    public class Session
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        public DateTime? EndTime { get; set; }

        [Required]
        public List<Round> Rounds { get; set; } = new();


        //Relacion muchos a muchos, User => Sessions
        [Required]
        public List<User> Players { get; set; } = new();

    }
}
