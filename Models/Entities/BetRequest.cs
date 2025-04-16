namespace RouletteTechTest.API.Models.Entities
{
    public class BetRequest
    {
        public string UserName { get; set; } = null!;
        public BetType Type { get; set; }
        public decimal Amount { get; set; }
        public Guid SessionId { get; set; }
        
        // Propiedades específicas según el tipo de apuesta
        public string? SelectedColor { get; set; }
        public string? SelectedParity { get; set; }
        public int? SelectedNumber { get; set; }
    }
}
