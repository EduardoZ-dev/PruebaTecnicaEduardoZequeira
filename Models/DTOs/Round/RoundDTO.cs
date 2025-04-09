namespace RouletteTechTest.API.Models.DTOs.Round
{
    public class RoundDTO
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = null!;
        public int RoundNumber { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public Guid SessionId { get; set; }
    }
}
