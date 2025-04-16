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

        public SessionGame? GetSession(Guid sessionId)
        {
            _sessions.TryGetValue(sessionId, out var session);
            return session;
        }

        public async Task SaveSessionAsync(Guid sessionId, decimal updatedBalance)
        {
            if (!_sessions.ContainsKey(sessionId))
                throw new ArgumentException("Sesión no encontrada");

            var session = _sessions[sessionId];
            session.CurrentBalance = updatedBalance;

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
