namespace RouletteTechTest.API.Models
{
    public class BetRequest
    {
        public string SessionId { get; set; }
        public BetData Bet { get; set; }
    }

    public class BetData
    {
        public string UserName { get; set; }
        public decimal BetAmount { get; set; }
        public string BetType { get; set; }
        public string BetValue { get; set; }
    }
} 