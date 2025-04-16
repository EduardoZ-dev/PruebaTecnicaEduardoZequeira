using RouletteTechTest.API.Models;
using RouletteTechTest.API.Models.Entities;

namespace RouletteTechTest.API.Services
{
    public interface ISessionService
    {
        SessionGame StartSession(string userName, decimal initialBalance);
        //Task<BetResponse> ProcessBet(BetRequest betRequest);
        SessionGame? GetSession(Guid sessionId);
        Task SaveSessionAsync(Guid sessionId, decimal updatedBalance);
    }
}
