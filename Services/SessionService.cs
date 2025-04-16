using RouletteTechTest.API.Data;
using RouletteTechTest.API.Models;
using RouletteTechTest.API.Models.Entities;

namespace RouletteTechTest.API.Services
{
    public class SessionService : ISessionService
    {
        // Almacenamiento en memoria de las sesiones activas
        private static readonly Dictionary<Guid, SessionGame> _sessions = new Dictionary<Guid, SessionGame>();
        private readonly IRouletteService _rouletteService;
        private readonly IUserRepository _userRepository;

        public SessionService(IRouletteService rouletteService, IUserRepository userRepository)
        {
            _rouletteService = rouletteService;
            _userRepository = userRepository;
        }

        public SessionGame StartSession(string userName, decimal initialBalance)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentException("El nombre de usuario no puede estar vacío");
            
            if (initialBalance <= 0)
                throw new ArgumentException("El saldo inicial debe ser mayor que cero");

            var session = new SessionGame
            {
                SessionId = Guid.NewGuid(),
                UserName = userName,
                CurrentBalance = initialBalance
            };
            _sessions[session.SessionId] = session;
            return session;
        }

        public async Task<BetResponse> ProcessBet(BetRequest betRequest)
        {
            Console.WriteLine($"DEBUG - ProcessBet - Recibiendo apuesta:");
            Console.WriteLine($"Tipo de apuesta (enum value): {(int)betRequest.Type}");
            Console.WriteLine($"Tipo de apuesta (name): {betRequest.Type}");
            Console.WriteLine($"Número seleccionado: {betRequest.SelectedNumber}");
            Console.WriteLine($"Color seleccionado: {betRequest.SelectedColor}");
            Console.WriteLine($"Paridad seleccionada: {betRequest.SelectedParity}");
            Console.WriteLine($"Monto: {betRequest.Amount}");

            if (!_sessions.ContainsKey(betRequest.SessionId))
                throw new ArgumentException("Sesión no encontrada");

            var session = _sessions[betRequest.SessionId];
            
            // Validaciones básicas
            if (betRequest == null)
                throw new ArgumentException("Apuesta inválida");

            if (string.IsNullOrWhiteSpace(betRequest.UserName))
                throw new ArgumentException("Nombre de usuario requerido");

            if (betRequest.Amount <= 0)
                throw new ArgumentException("Monto de apuesta inválido");

            if (session.CurrentBalance < betRequest.Amount)
                throw new ArgumentException("Saldo insuficiente");

            // Validación específica según el tipo de apuesta
            switch (betRequest.Type)
            {
                case BetType.Color:
                    if (string.IsNullOrEmpty(betRequest.SelectedColor))
                        throw new ArgumentException("Color no especificado");
                    break;
                case BetType.ParImpar:
                    if (string.IsNullOrEmpty(betRequest.SelectedParity))
                        throw new ArgumentException("Paridad no especificada");
                    break;
                case BetType.Numero:
                    if (!betRequest.SelectedNumber.HasValue)
                        throw new ArgumentException("Número no especificado");
                    if (betRequest.SelectedNumber.Value < 0 || betRequest.SelectedNumber.Value > 36)
                        throw new ArgumentException("Número inválido");
                    break;
                case BetType.NumeroColor:
                    Console.WriteLine("DEBUG - Validando apuesta de número y color:");
                    
                    if (!betRequest.SelectedNumber.HasValue)
                    {
                        Console.WriteLine("Error: Número no especificado");
                        throw new ArgumentException("Número no especificado");
                    }
                    
                    var number = betRequest.SelectedNumber.Value;
                    Console.WriteLine($"Número a validar: {number}");
                    
                    if (number < 0 || number > 36)
                    {
                        Console.WriteLine($"Error: Número {number} fuera de rango (0-36)");
                        throw new ArgumentException("Número inválido");
                    }

                    if (string.IsNullOrEmpty(betRequest.SelectedColor))
                    {
                        Console.WriteLine("Error: Color no especificado");
                        throw new ArgumentException("Color no especificado");
                    }

                    var normalizedColor = RouletteColors.NormalizeColor(betRequest.SelectedColor);
                    Console.WriteLine($"Color normalizado: {normalizedColor}");
                    
                    if (!RouletteColors.IsValidColorForNumber(number, normalizedColor))
                    {
                        Console.WriteLine($"Error: Color {normalizedColor} no válido para el número {number}");
                        throw new ArgumentException("Color no válido para el número seleccionado");
                    }
                    
                    // Asignar el color normalizado de vuelta al betRequest
                    betRequest.SelectedColor = normalizedColor;
                    break;
                default:
                    throw new ArgumentException("Tipo de apuesta inválido");
            }

            Console.WriteLine("\nDEBUG - SessionService.ProcessBet - Antes de Spin:");
            Console.WriteLine($"Color seleccionado (validado): {betRequest.SelectedColor}");

            var spinResult = _rouletteService.Spin();

            Console.WriteLine("\nDEBUG - SessionService.ProcessBet - Antes de CalculatePrize:");
            Console.WriteLine($"Color seleccionado (final): {betRequest.SelectedColor}");
            Console.WriteLine($"Número resultado: {spinResult.Number}");
            Console.WriteLine($"Color resultado: {spinResult.Color}");

            var prize = _rouletteService.CalculatePrize(betRequest, spinResult);

            var betResult = new BetResult
            {
                SpinResult = spinResult,
                Prize = prize,
                Message = prize > 0
                    ? $"¡Felicidades! Has ganado {prize:C}."
                    : $"Lo siento, has perdido {Math.Abs(prize):C}.",
                NewBalance = session.CurrentBalance + prize
            };

            session.CurrentBalance += prize;
            session.BetHistory.Add(betResult);

            return new BetResponse
            {
                BetResult = betResult,
                SessionId = session.SessionId
            };
        }

        public SessionGame? GetSession(Guid sessionId)
        {
            _sessions.TryGetValue(sessionId, out var session);
            return session;
        }

        public async Task SaveSessionAsync(Guid sessionId)
        {
            if (!_sessions.ContainsKey(sessionId))
                throw new ArgumentException("Sesión no encontrada");

            var session = _sessions[sessionId];

            var user = await _userRepository.GetUserByNameAsync(session.UserName);
            if (user == null)
            {
                user = new User { UserName = session.UserName, Balance = session.CurrentBalance };
                await _userRepository.AddUserAsync(user);
            }
            else
            {
                user.Balance = session.CurrentBalance;
                await _userRepository.UpdateUserAsync(user);
            }
            await _userRepository.SaveChangesAsync();

            _sessions.Remove(sessionId);
        }
    }
}
