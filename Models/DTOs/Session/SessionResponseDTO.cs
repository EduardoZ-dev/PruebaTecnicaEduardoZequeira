namespace RouletteTechTest.API.Models.DTOs.Session
{
    public class SessionResponseDTO
    {
        public Guid Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public decimal NetProfit { get; set; }
    }
}
