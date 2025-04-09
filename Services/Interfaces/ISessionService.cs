using RouletteTechTest.API.Models.DTOs.Session;

namespace RouletteTechTest.API.Services.Interfaces
{
    public interface ISessionService
    {
        Task<IEnumerable<SessionResponseDTO>> GetAllSessionsAsync();
        Task<SessionResponseDTO> CreateSessionAsync(SessionCreateDTO createDto);
        Task<SessionResponseDTO> AddPlayersToSessionAsync(string userName, SessionAddPlayersDTO addPlayersDto);
        Task EndSessionAsync(Guid sessionId);
        Task<SessionResponseDTO> GetSessionByIdAsync(Guid id);
    }
}
