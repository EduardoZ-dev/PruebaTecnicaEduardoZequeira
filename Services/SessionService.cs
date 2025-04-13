using RouletteTechTest.API.Data;
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
            var session = new SessionGame
            {
                SessionId = Guid.NewGuid(),
                UserName = userName,
                CurrentBalance = initialBalance
            };
            _sessions[session.SessionId] = session;
            return session;
        }

        public BetResult ProcessBet(Guid sessionId, BetRequest betRequest)
        {
            if (!_sessions.ContainsKey(sessionId))
            {
                throw new Exception("Sesión no encontrada.");
            }
            var session = _sessions[sessionId];

            var spinResult = _rouletteService.Spin();
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

            return betResult;
        }

        public SessionGame? GetSession(Guid sessionId)
        {
            _sessions.TryGetValue(sessionId, out var session);
            return session;
        }

        public async Task SaveSessionAsync(Guid sessionId)
        {
            if (!_sessions.ContainsKey(sessionId))
            {
                throw new Exception("Sesión no encontrada.");
            }
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
