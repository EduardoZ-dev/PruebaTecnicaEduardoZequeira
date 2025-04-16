using RouletteTechTest.API.Models.Entities;

namespace RouletteTechTest.API.Models
{
    public class BetResponse
    {
        public BetResult BetResult { get; set; }
        public Guid SessionId { get; set; }
    }
} 