namespace RouletteTechTest.API.Models.DTOs.Session
{
    public class SessionResponseDTO
    {
        public Guid Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public List<string> PlayerNames { get; set; } = new();
        public int RoundCount { get; set; }
    }
}
