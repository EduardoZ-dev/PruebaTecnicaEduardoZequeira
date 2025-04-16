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
            Console.WriteLine($"DEBUG - Spin:");
            Console.WriteLine($"Número generado: {number}");
            string color = RouletteColors.GetColorForNumber(number);
            Console.WriteLine($"Color asignado: {color}");
            return new SpinResult { Number = number, Color = color };
        }

        public decimal CalculatePrize(BetRequest bet, SpinResult spinResult)
        {
            if (bet == null || spinResult == null)
                return -bet.Amount;

            Console.WriteLine($"\nDEBUG - CalculatePrize:");
            Console.WriteLine($"Tipo de apuesta: {bet.Type}");
            Console.WriteLine($"Color apostado (original): {bet.SelectedColor}");
            Console.WriteLine($"Número resultado: {spinResult.Number}");

            switch (bet.Type)
            {
                case BetType.Color:
                    string normalizedBetColor = RouletteColors.NormalizeColor(bet.SelectedColor);
                    string resultColor = RouletteColors.GetColorForNumber(spinResult.Number);
                    bool isWinningBet = string.Equals(normalizedBetColor, resultColor, StringComparison.OrdinalIgnoreCase);

                    Console.WriteLine($"DEBUG - Apuesta por color:");
                    Console.WriteLine($"Color apostado (original): {bet.SelectedColor}");
                    Console.WriteLine($"Color apostado (normalizado): {normalizedBetColor}");
                    Console.WriteLine($"Color resultado: {resultColor}");
                    Console.WriteLine($"¿Colores coinciden? {isWinningBet}");
                    Console.WriteLine($"Número resultado: {spinResult.Number}");

                    if (isWinningBet)
                        return bet.Amount * 2m;
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
                        
                        Console.WriteLine($"DEBUG - Apuesta par/impar:");
                        Console.WriteLine($"Número: {spinResult.Number}");
                        Console.WriteLine($"Paridad resultado: {resultParity}");
                        Console.WriteLine($"Paridad apostada: {normalizedBetParity}");
                        Console.WriteLine($"¿Paridad coincide? {parityMatch}");
                        Console.WriteLine($"¿Color coincide? {colorMatch}");
                        
                        if (parityMatch && colorMatch)
                            return bet.Amount * 4m;
                    }
                    break;

                case BetType.Numero:
                    if (bet.SelectedNumber == spinResult.Number)
                    {
                        return bet.Amount * 35m;
                    }
                    break;

                case BetType.NumeroColor:
                    if (bet.SelectedNumber == spinResult.Number)
                    {
                        string betColor = RouletteColors.NormalizeColor(bet.SelectedColor);
                        string spinResultColor = RouletteColors.GetColorForNumber(spinResult.Number);
                        bool colorMatch = string.Equals(betColor, spinResultColor, StringComparison.OrdinalIgnoreCase);

                        Console.WriteLine($"DEBUG - Apuesta número y color:");
                        Console.WriteLine($"Número apostado: {bet.SelectedNumber}");
                        Console.WriteLine($"Número resultado: {spinResult.Number}");
                        Console.WriteLine($"Color apostado: {betColor}");
                        Console.WriteLine($"Color resultado: {spinResultColor}");
                        Console.WriteLine($"¿Colores coinciden? {colorMatch}");

                        if (colorMatch)
                            return bet.Amount * 36m;
                        else
                            return bet.Amount * 35m;
                    }
                    break;
            }

            return -bet.Amount;
        }

        public async Task<BetResponse> ProcessBet(BetRequest betRequest)
        {
            if (betRequest == null)
                throw new ArgumentException("Apuesta inválida");

            Console.WriteLine("\nDEBUG - ProcessBetAsync - Inicio:");
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
                        Console.WriteLine("ERROR - Color no especificado en la apuesta");
                        throw new ArgumentException("Color no especificado");
                    }
                    // Normalizar el color aquí para asegurarnos que es válido
                    string normalizedColor = RouletteColors.NormalizeColor(betRequest.SelectedColor);
                    if (string.IsNullOrWhiteSpace(normalizedColor))
                    {
                        Console.WriteLine($"ERROR - Color inválido después de normalización: {betRequest.SelectedColor}");
                        throw new ArgumentException("Color inválido");
                    }
                    // Asignar el color normalizado de vuelta al request
                    betRequest.SelectedColor = normalizedColor;
                    Console.WriteLine($"Color normalizado: {normalizedColor}");
                    break;
                case BetType.ParImpar:
                    if (string.IsNullOrWhiteSpace(betRequest.SelectedParity))
                        throw new ArgumentException("Paridad no especificada");
                    break;
                case BetType.Numero:
                    if (!betRequest.SelectedNumber.HasValue || betRequest.SelectedNumber < 0 || betRequest.SelectedNumber > 36)
                        throw new ArgumentException("Número inválido");
                    break;
                default:
                    throw new ArgumentException("Tipo de apuesta inválido");
            }

            var user = await _userRepository.GetUserByNameAsync(betRequest.UserName);
            if (user == null)
                throw new ArgumentException("Usuario no encontrado");

            if (user.Balance < betRequest.Amount)
                throw new ArgumentException("Saldo insuficiente");

            Console.WriteLine("\nDEBUG - ProcessBetAsync - Antes de Spin:");
            Console.WriteLine($"Color seleccionado (validado): {betRequest.SelectedColor}");

            SpinResult spinResult = Spin();

            Console.WriteLine("\nDEBUG - ProcessBetAsync - Antes de CalculatePrize:");
            Console.WriteLine($"Color seleccionado (final): {betRequest.SelectedColor}");
            Console.WriteLine($"Número resultado: {spinResult.Number}");
            Console.WriteLine($"Color resultado: {spinResult.Color}");

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

        /*public decimal CalculatePrize(Models.Entities.BetRequest bet, SpinResult spinResult)
        {
            throw new NotImplementedException();
        }

        public Task<BetResult> ProcessBetAsync(Models.Entities.BetRequest betRequest)
        {
            throw new NotImplementedException();
        }*/
    }
}
