using RouletteTechTest.API.Models.Entities;

namespace RouletteTechTest.API.Services
{
    public interface ISessionService
    {
        SessionGame StartSession(string userName, decimal initialBalance);
        BetResult ProcessBet(Guid sessionId, BetRequest betRequest);
        SessionGame? GetSession(Guid sessionId);
        Task SaveSessionAsync(Guid sessionId);
    }
}
