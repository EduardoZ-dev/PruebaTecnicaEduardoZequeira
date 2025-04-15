namespace RouletteTechTest.API.Models.Entities
{
    public class BetRequest
    {
        public string UserName { get; set; } = null!;
        public BetType BetType { get; set; }
        public decimal BetAmount { get; set; }
        
        // Propiedades específicas según el tipo de apuesta
        public string? SelectedColor { get; set; }
        public string? SelectedParity { get; set; }
        public int? SelectedNumber { get; set; }

        // Método para validar que la apuesta tenga los datos necesarios según su tipo
        public bool IsValid()
        {
            if (string.IsNullOrEmpty(UserName) || BetAmount <= 0)
                return false;

            return BetType switch
            {
                BetType.Color => !string.IsNullOrEmpty(SelectedColor),
                BetType.ParImpar => !string.IsNullOrEmpty(SelectedParity),
                BetType.Numero => SelectedNumber.HasValue && SelectedNumber >= 0 && SelectedNumber <= 36,
                _ => false
            };
        }
    }
}
