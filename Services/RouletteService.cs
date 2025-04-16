using RouletteTechTest.API.Data;
using RouletteTechTest.API.Models;
using RouletteTechTest.API.Models.Entities;

namespace RouletteTechTest.API.Services
{
    public class RouletteService : IRouletteService
    {
        private readonly IUserRepository _userRepository;
        private readonly Random _random;

        public RouletteService(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _random = new Random();
        }

        public SpinResult Spin()
        {
            int number = _random.Next(0, 37);
            Console.WriteLine($"Número generado: {number}");
            string color = RouletteColors.GetColorForNumber(number);
            Console.WriteLine($"Color asignado: {color}");
            return new SpinResult { Number = number, Color = color };
        }

        public decimal CalculatePrize(BetRequest bet, SpinResult spinResult)
        {
            if (bet == null || spinResult == null)
                return -bet.Amount;

            switch (bet.Type)
            {
                case BetType.Color:
                    string normalizedBetColor = RouletteColors.NormalizeColor(bet.SelectedColor);
                    string resultColor = RouletteColors.GetColorForNumber(spinResult.Number);
                    bool isWinningBet = string.Equals(normalizedBetColor, resultColor, StringComparison.OrdinalIgnoreCase);

                    if (isWinningBet)
                        return bet.Amount * 0.5m;
                    break;

                case BetType.ParImpar:
                    if (spinResult.Number != 0)
                    {
                        string resultParity = (spinResult.Number % 2 == 0) ? "par" : "impar";
                        string normalizedBetParity = bet.SelectedParity?.ToLower();
                        
                        bool parityMatch = string.Equals(normalizedBetParity, resultParity, StringComparison.OrdinalIgnoreCase);
                        bool colorMatch = false;

                        string betColor = RouletteColors.NormalizeColor(bet.SelectedColor);
                        if (betColor == "negro")
                        {
                            colorMatch = RouletteColors.BlackNumbers.Contains(spinResult.Number);
                        }
                        else if (betColor == "rojo")
                        {
                            colorMatch = RouletteColors.RedNumbers.Contains(spinResult.Number);
                        }
                        
                        if (parityMatch && colorMatch)
                            return bet.Amount * 1m;
                    }
                    break;

                case BetType.NumeroColor:
                    if (bet.SelectedNumber == spinResult.Number)
                    {
                        string betColor = RouletteColors.NormalizeColor(bet.SelectedColor);
                        string spinResultColor = RouletteColors.GetColorForNumber(spinResult.Number);
                        bool colorMatch = string.Equals(betColor, spinResultColor, StringComparison.OrdinalIgnoreCase);

                        if (colorMatch)
                            return bet.Amount * 3m;
                    }
                    break;
            }

            return -bet.Amount;
        }

        public async Task<BetResponse> ProcessBet(BetRequest betRequest)
        {
            if (betRequest == null)
                throw new ArgumentException("Apuesta inválida");

            Console.WriteLine($"Tipo de apuesta: {betRequest.Type}");
            Console.WriteLine($"Color seleccionado (original): {betRequest.SelectedColor ?? "null"}");
            Console.WriteLine($"Monto: {betRequest.Amount}");

            if (string.IsNullOrWhiteSpace(betRequest.UserName))
                throw new ArgumentException("Nombre de usuario requerido");

            if (betRequest.Amount <= 0)
                throw new ArgumentException("Monto de apuesta inválido");

            // Validación específica según el tipo de apuesta
            switch (betRequest.Type)
            {
                case BetType.Color:
                    if (string.IsNullOrWhiteSpace(betRequest.SelectedColor))
                    {
                        throw new ArgumentException("Color no especificado");
                    }
                    // Normalizar el color aquí para asegurarnos que es válido
                    string normalizedColor = RouletteColors.NormalizeColor(betRequest.SelectedColor);
                    if (string.IsNullOrWhiteSpace(normalizedColor))
                    {
                        throw new ArgumentException("Color inválido");
                    }
                    // Asignar el color normalizado de vuelta al request
                    betRequest.SelectedColor = normalizedColor;

                    break;
                case BetType.ParImpar:
                    if (string.IsNullOrWhiteSpace(betRequest.SelectedParity))
                        throw new ArgumentException("Paridad no especificada");
                    break;

                case BetType.Numero:
                    if (!betRequest.SelectedNumber.HasValue || betRequest.SelectedNumber < 0 || betRequest.SelectedNumber > 36)
                        throw new ArgumentException("Número inválido");
                    break;

                case BetType.NumeroColor:
                    if(!betRequest.SelectedNumber.HasValue || betRequest.SelectedNumber < 0 || betRequest.SelectedNumber > 36)
                    {
                        throw new ArgumentException("Número inválido");
                    }
                    if (string.IsNullOrWhiteSpace(betRequest.SelectedColor))
                    {
                        throw new ArgumentException("Color no especificado");
                    }
                    string normalizeColor = RouletteColors.NormalizeColor(betRequest.SelectedColor);
                    if (string.IsNullOrWhiteSpace(normalizeColor))
                    {
                        throw new ArgumentException("Color inválido");
                    }
                    string expectedColor = RouletteColors.GetColorForNumber(betRequest.SelectedNumber.Value);
                    if (!string.Equals(normalizeColor, expectedColor, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new ArgumentException("El color no corresponde al número seleccionado");
                    }
                    // Asignar el color normalizado de vuelta al request
                    betRequest.SelectedColor = normalizeColor;
                    break;

                default:
                    throw new ArgumentException("Tipo de apuesta inválido");
            }

            var user = await _userRepository.GetUserByNameAsync(betRequest.UserName);
            if (user == null)
                throw new ArgumentException("Usuario no encontrado");

            if (user.Balance < betRequest.Amount)
                throw new ArgumentException("Saldo insuficiente");

            SpinResult spinResult = Spin();

            decimal prize = CalculatePrize(betRequest, spinResult);

            user.Balance += prize;
            await _userRepository.SaveChangesAsync();

            string message = prize > 0
                ? $"¡Felicidades! Has ganado {prize:C}."
                : $"Lo siento, has perdido {Math.Abs(prize):C}.";

            var betResult = new BetResult
            {
                SpinResult = spinResult,
                Prize = prize,
                Message = message,
                NewBalance = user.Balance
            };

            return new BetResponse
            {
                BetResult = betResult,
                SessionId = betRequest.SessionId
            };
        }

        public async Task UpdateUserBalanceAsync(string userName, decimal amount)
        {
            var user = await _userRepository.GetUserByNameAsync(userName);
            if (user == null)
            {
                user = new User { UserName = userName, Balance = amount };
                await _userRepository.AddUserAsync(user);
            }
            else
            {
                user.Balance = amount;
                await _userRepository.UpdateUserAsync(user);
            }
            await _userRepository.SaveChangesAsync();
        }
    }
}
