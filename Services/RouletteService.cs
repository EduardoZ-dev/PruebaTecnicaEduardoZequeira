using RouletteTechTest.API.Data;
using RouletteTechTest.API.Models.Entities;

namespace RouletteTechTest.API.Services
{
    public class RouletteService : IRouletteService
    {
        private readonly IUserRepository _userRepository;
        private readonly Random _random;

        public RouletteService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _random = new Random();
        }

        public SpinResult Spin()
        {
            int number = _random.Next(0, 37);
            string color = _random.Next(0, 2) == 0 ? "rojo" : "negro";
            return new SpinResult { Number = number, Color = color };
        }

        public decimal CalculatePrize(BetRequest betRequest, SpinResult spinResult)
        {
            decimal prize = 0;
            bool win = false;

            switch (betRequest.BetType)
            {
                case BetType.Color:
                    if (string.Equals(betRequest.SelectedColor, spinResult.Color, StringComparison.OrdinalIgnoreCase))
                    {
                        prize = betRequest.BetAmount * 0.5m;
                        win = true;
                    }
                    break;
                case BetType.ParImpar:
                    if (string.Equals(betRequest.SelectedColor, spinResult.Color, StringComparison.OrdinalIgnoreCase)
                       && spinResult.Number != 0)
                    {
                        string resultParity = (spinResult.Number % 2 == 0) ? "par" : "impar";
                        if (string.Equals(betRequest.SelectedParity, resultParity, StringComparison.OrdinalIgnoreCase))
                        {
                            prize = betRequest.BetAmount;
                            win = true;
                        }
                    }
                    break;
                case BetType.Numero:
                    if (betRequest.SelectedNumber.HasValue &&
                        betRequest.SelectedNumber.Value == spinResult.Number &&
                        string.Equals(betRequest.SelectedColor, spinResult.Color, StringComparison.OrdinalIgnoreCase))
                    {
                        prize = betRequest.BetAmount * 3;
                        win = true;
                    }
                    break;
            }
            if (!win)
            {
                prize = -betRequest.BetAmount;
            }
            return prize;
        }

        public async Task<BetResult> ProcessBetAsync(BetRequest betRequest)
        {
            SpinResult spinResult = Spin();
            decimal prize = CalculatePrize(betRequest, spinResult);

            var user = await _userRepository.GetUserByNameAsync(betRequest.UserName);
            if (user == null)
            {
                user = new User { UserName = betRequest.UserName, Balance = 0 };
                await _userRepository.AddUserAsync(user);
            }
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
