using RouletteTechTest.API.Data.Common;
using RouletteTechTest.API.Models.DTOs.Session;
using RouletteTechTest.API.Models.Entities;
using RouletteTechTest.API.Services.Interfaces;

namespace RouletteTechTest.API.Services.Implementations
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _uow;

        public SessionService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<IEnumerable<SessionResponseDTO>> GetAllSessionsAsync()
        {
            var sessions = await _uow.Sessions.GetAllAsync();
            return sessions.Select(MapToResponseDTO).ToList();
        }

        public async Task<SessionResponseDTO> CreateSessionAsync(SessionCreateDTO createDto)
        {
            await _uow.BeginTransactionAsync();
            try
            {
                var players = new List<User>();

                foreach (var userName in createDto.PlayerNames)
                {
                    var user = await _uow.Users.GetByNameAsync(userName)
                        ?? throw new KeyNotFoundException($"Usuario '{userName}' no existe");

                    if (await _uow.Users.HasActiveSessionAsync(user.Id))
                        throw new InvalidOperationException($"El usuario {userName} ya está en una sesión activa");

                    players.Add(user);
                }

                var session = new Session { Players = players };
                await _uow.Sessions.AddAsync(session);
                await _uow.CommitAsync();

                return MapToResponseDTO(session);
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }

        public async Task<SessionResponseDTO> AddPlayersToSessionAsync(Guid sessionId, SessionAddPlayersDTO addPlayersDto)
        {
            await _uow.BeginTransactionAsync();
            try
            {
                var session = await _uow.Sessions.GetByIdAsync(sessionId)
                    ?? throw new KeyNotFoundException("Sesión no encontrada");

                if (session.EndTime != null)
                    throw new InvalidOperationException("No se pueden agregar jugadores a una sesión finalizada");

                var playersToAdd = new List<User>();
                foreach (var userName in addPlayersDto.PlayerNames)
                {
                    var user = await _uow.Users.GetByNameAsync(userName)
                        ?? throw new KeyNotFoundException($"Usuario '{userName}' no existe");

                    if (await _uow.Users.HasActiveSessionAsync(user.Id))
                        throw new InvalidOperationException($"El usuario {userName} ya está en una sesión activa");

                    if (!session.Players.Any(p => p.Id == user.Id))
                        playersToAdd.Add(user);
                }

                await _uow.Sessions.AddPlayersToSession(sessionId, playersToAdd);
                await _uow.SaveChangesAsync();
                await _uow.CommitAsync();

                return MapToResponseDTO(await _uow.Sessions.GetByIdAsync(sessionId));
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }

        public async Task EndSessionAsync(Guid sessionId)
        {
            await _uow.BeginTransactionAsync();
            try
            {
                var session = await _uow.Sessions.GetByIdAsync(sessionId)
                    ?? throw new KeyNotFoundException("Sesión no encontrada");

                session.EndTime = DateTime.UtcNow;
                _uow.Sessions.Update(session);
                await _uow.SaveChangesAsync();
                await _uow.CommitAsync();
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }

        public async Task<SessionResponseDTO> GetSessionByIdAsync(Guid id)
        {
            var session = await _uow.Sessions.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Sesión no encontrada");
            return MapToResponseDTO(session);
        }

        private SessionResponseDTO MapToResponseDTO(Session session)
        {
            return new SessionResponseDTO
            {
                Id = session.Id,
                StartTime = session.StartTime,
                EndTime = session.EndTime,
                PlayerNames = session.Players.Select(p => p.UserName).ToList(),
                RoundCount = session.Rounds.Count
            };
        }
    }
}
