namespace RouletteTechTest.API.Models.Entities
{
    public class SessionGame
    {
        public Guid SessionId { get; set; }
        public string UserName { get; set; } = null!;
        public decimal CurrentBalance { get; set; }
        public List<BetResult> BetHistory { get; set; } = new List<BetResult>();
    }
}
