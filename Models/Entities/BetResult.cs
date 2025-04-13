namespace RouletteTechTest.API.Models.Entities
{
    public class BetResult
    {
        public SpinResult SpinResult { get; set; }
        public decimal Prize { get; set; }
        public string Message { get; set; } = null!;
        public decimal NewBalance { get; set; }
    }
}
