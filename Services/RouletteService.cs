using RouletteTechTest.API.Data;
using RouletteTechTest.API.Models.Entities;

namespace RouletteTechTest.API.Services
{
    public class RouletteService : IRouletteService
    {
        private readonly IUserRepository _userRepository;
        private readonly Random _random;

        // Números rojos en la ruleta europea
        private readonly HashSet<int> redNumbers = new HashSet<int> { 1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36 };

        private string GetColorForNumber(int number)
        {
            if (number == 0) return "verde";
            return redNumbers.Contains(number) ? "rojo" : "negro";
        }

        private string NormalizeColor(string color)
        {
            return color?.ToLower() switch
            {
                "red" => "rojo",
                "black" => "negro",
                "green" => "verde",
                _ => color?.ToLower() ?? ""
            };
        }

        public RouletteService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _random = new Random();
        }

        public SpinResult Spin()
        {
            int number = _random.Next(0, 37);
            string color = GetColorForNumber(number);
            return new SpinResult { Number = number, Color = color };
        }

        public decimal CalculatePrize(BetRequest bet, SpinResult spinResult)
        {
            if (bet == null || !bet.IsValid() || spinResult == null)
                return -bet.BetAmount;

            string normalizedBetColor = NormalizeColor(bet.SelectedColor);
            string resultColor = spinResult.Color?.ToLower() ?? "";

            switch (bet.BetType)
            {
                case BetType.Color:
                    if (string.Equals(normalizedBetColor, resultColor, StringComparison.OrdinalIgnoreCase))
                        return bet.BetAmount * 0.5m;
                    break;

                case BetType.ParImpar:
                    if (spinResult.Number != 0 && // El cero no cuenta para par/impar
                        string.Equals(GetColorForNumber(spinResult.Number), resultColor, StringComparison.OrdinalIgnoreCase))
                    {
                        string resultParity = (spinResult.Number % 2 == 0) ? "par" : "impar";
                        if (string.Equals(bet.SelectedParity?.ToLower(), resultParity, StringComparison.OrdinalIgnoreCase))
                            return bet.BetAmount;
                    }
                    break;

                case BetType.Numero:
                    if (bet.SelectedNumber == spinResult.Number)
                        return bet.BetAmount * 3;
                    break;
            }

            return -bet.BetAmount;
        }

        public async Task<BetResult> ProcessBetAsync(BetRequest betRequest)
        {
            if (!betRequest.IsValid())
                throw new ArgumentException("Apuesta inválida");

            var user = await _userRepository.GetUserByNameAsync(betRequest.UserName);
            if (user == null)
                throw new ArgumentException("Usuario no encontrado");

            if (user.Balance < betRequest.BetAmount)
                throw new ArgumentException("Saldo insuficiente");

            SpinResult spinResult = Spin();
            decimal prize = CalculatePrize(betRequest, spinResult);

            user.Balance += prize;
            await _userRepository.SaveChangesAsync();

            string message = prize > 0
                ? $"¡Felicidades! Has ganado {prize:C}."
                : $"Lo siento, has perdido {Math.Abs(prize):C}.";

            return new BetResult
            {
                SpinResult = spinResult,
                Prize = prize,
                Message = message,
                NewBalance = user.Balance
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
