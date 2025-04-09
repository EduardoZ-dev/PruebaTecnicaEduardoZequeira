using RouletteTechTest.API.Data.Common;
using RouletteTechTest.API.Data.Repositories;
using RouletteTechTest.API.Models.DTOs.Session;
using RouletteTechTest.API.Models.Entities;
using RouletteTechTest.API.Services.Interfaces;

namespace RouletteTechTest.API.Services.Implementations
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _uow;
        private readonly IUserRepository _userRepository;
        private readonly ISessionRepository _sessionRepository;

        public SessionService(
            IUnitOfWork uow,
            IUserRepository userRepository,
            ISessionRepository sessionRepository)
        {
            _uow = uow;
            _userRepository = userRepository;
            _sessionRepository = sessionRepository;
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
                // 1. Validar usuarios en una sola consulta
                var users = (await _userRepository.GetAllByNamesAsync(createDto.PlayerNames)).ToList();

                // 2. Verificar usuarios faltantes
                var missingUsers = createDto.PlayerNames
                    .Except(users.Select(u => u.UserName))
                    .ToList();

                if (missingUsers.Any())
                {
                    throw new KeyNotFoundException(
                        $"Usuarios no registrados: {string.Join(", ", missingUsers)}"
                    );
                }

                // 3. Verificar sesiones activas
                var activeUsers = await _userRepository.GetUsersWithActiveSessionsAsync(
                    users.Select(u => u.Id)
                );

                if (activeUsers.Any())
                {
                    throw new InvalidOperationException(
                        $"Usuarios con sesión activa: {string.Join(", ", activeUsers.Select(u => u.UserName))}"
                    );
                }

                // 4. Crear nueva sesión
                var newSession = new Session
                {
                    Players = users,
                    StartTime = DateTime.UtcNow,
                    Rounds = new List<Round>()
                };

                // 5. Persistir en base de datos
                await _sessionRepository.AddAsync(newSession);
                await _uow.CommitAsync();

                // 6. Mapear a DTO
                return MapToResponseDTO(newSession);
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }

        public async Task<SessionResponseDTO> AddPlayersToSessionAsync(string userName, SessionAddPlayersDTO addPlayersDto)
        {
            await _uow.BeginTransactionAsync();
            try
            {
                // 1. Obtener sesión activa del usuario
                var activeSession = await _uow.Sessions.GetActiveSessionByUserAsync(userName)
                    ?? throw new KeyNotFoundException("No tienes una sesión activa");

                // 2. Validar que la sesión está abierta
                if (activeSession.EndTime != null)
                    throw new InvalidOperationException("No se pueden agregar jugadores a una sesión finalizada");

                // 3. Validar y obtener usuarios a agregar
                var playersToAdd = new List<User>();
                foreach (var playerName in addPlayersDto.PlayerNames)
                {
                    // Validar existencia del usuario
                    var user = await _uow.Users.GetByNameAsync(playerName)
                        ?? throw new KeyNotFoundException($"Jugador '{playerName}' no encontrado");

                    // Validar que no esté ya en la sesión
                    if (activeSession.Players.Any(p => p.UserName == playerName))
                        continue;

                    // Validar que no tenga otra sesión activa
                    if (await _uow.Sessions.GetActiveSessionByUserAsync(playerName) != null)
                        throw new InvalidOperationException($"El jugador {playerName} ya está en otra sesión activa");

                    playersToAdd.Add(user);
                }

                // 4. Agregar jugadores a la sesión
                await _uow.Sessions.AddPlayersToSession(activeSession.Id, playersToAdd.Select(p => p.UserName));
                await _uow.SaveChangesAsync();

                // 5. Actualizar la sesión desde la base de datos
                var updatedSession = await _uow.Sessions.GetByIdAsync(activeSession.Id);

                // 6. Commit de la transacción
                await _uow.CommitAsync();

                // 7. Mapear a DTO
                return MapToResponseDTO(updatedSession);
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                throw new ApplicationException("Error al agregar jugadores a la sesión", ex);
            }
        }

        // Método de mapeo a DTO
        /*private SessionResponseDTO MapToResponseDTO(Session session)
        {
            return new SessionResponseDTO
            {
                Id = session.Id,
                StartTime = session.StartTime,
                EndTime = session.EndTime,
                PlayerNames = session.Players.Select(p => p.UserName).ToList(),
                RoundCount = session.Rounds?.Count ?? 0
            };
        }*/

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
                PlayerNames = session.Players
                    .OrderBy(p => p.UserName)
                    .Select(p => p.UserName)
                    .ToList(),
                RoundCount = session.Rounds?.Count ?? 0
            };
        }
    }
}
