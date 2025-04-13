namespace RouletteTechTest.API.Models.Entities
{
    public class BetRequest
    {
        public string UserName { get; set; } = null!;
        public BetType BetType { get; set; }
        public decimal BetAmount { get; set; }

        //apuestas de tipo Color y Número.
        public string SelectedColor { get; set; } = null!;

        //apuestas de ParImpar.
        public string SelectedParity { get; set; } = null!;

        //apuestas de Número.
        public int? SelectedNumber { get; set; }
    }
}
