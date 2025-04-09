using RouletteTechTest.API.Data.Common;
using RouletteTechTest.API.Models.DTOs.Bet;
using RouletteTechTest.API.Models.DTOs.Common;
using RouletteTechTest.API.Models.DTOs.Round;
using RouletteTechTest.API.Models.Entities;
using RouletteTechTest.API.Models.Enums;
using RouletteTechTest.API.Services.Interfaces;

namespace RouletteTechTest.API.Services.Implementations
{
    public class RoundService : IRoundService
    {
        private readonly IUnitOfWork _uow;
        private readonly IBetCalculator _betCalculator;
        private readonly Random _random = new Random();

        public RoundService(IUnitOfWork uow, IBetCalculator betCalculator)
        {
            _uow = uow;
            _betCalculator = betCalculator;
        }

        public async Task<IEnumerable<RoundResponseDTO>> GetAllRoundAsync()
        {
            var sessions = await _uow.Rounds.GetAllRoundsAsync();
            return sessions.Select(MapToResponseDTO).ToList();
        }

        public async Task<RoundDTO> GetCurrentActiveRoundAsync(string userName)
        {
            var sessions = await _uow.Sessions.GetByNameAsync(userName);
            var activeSession = sessions.FirstOrDefault(s => s.EndTime == null);

            if (activeSession == null)
                throw new InvalidOperationException("No hay sesión activa para este usuario");

            var activeRound = await _uow.Rounds.GetActiveRoundAsync(activeSession.Id);

            if (activeRound == null)
                throw new InvalidOperationException("No hay ronda activa en esta sesión");

            return new RoundDTO
            {
                Id = activeRound.Id,
                UserName = activeRound.UserName,
                RoundNumber = activeRound.RoundNumber,
                StartTime = activeRound.StartTime,
                EndTime = activeRound.EndTime,
                SessionId = activeRound.SessionId
            };
        }

        private RoundResponseDTO MapToResponseDTO(Round round)
        {
            return new RoundResponseDTO
            {
                Id = round.Id,
                RoundNumber = round.RoundNumber,
                StartTime = round.StartTime,
                EndTime = round.EndTime,
                SessionId = round.SessionId,
                Result = round.Result != null ? new SpinResultDTO
                {
                    ResultNumber = round.Result.ResultNumber,
                    Color = round.Result.Color,
                    Parity = round.Result.Parity,
                    SpinTime = round.Result.SpinTime
                } : null,
                Bets = round.Bets.Select(b => new BetDTO
                {
                    Id = b.Id,
                    UserName = b.User?.UserName ?? b.UserName,
                    BetType = b.Type.ToString(),
                    BetValue = b.BetValue,
                    Amount = b.Amount,
                    TimeStamp = b.TimeStamp,
                    Outcome = b.Outcome.ToString(),
                    Prize = b.Prize
                }).ToList()
            };
        }

        public async Task<RoundDTO> StartRoundAsync(string userName)
        {
            var sessions = await _uow.Sessions.GetByNameAsync(userName);

            var activeSession = sessions.FirstOrDefault(s => s?.EndTime == null)
                ?? throw new InvalidOperationException("No hay sesiones activas para este usuario");

            var hasActiveRound = await _uow.Rounds.ExistsActiveRoundAsync(activeSession.Id);
            if (hasActiveRound)
                throw new InvalidOperationException("Ya existe una ronda activa en esta sesión");

            var newRound = new Round
            {
                Id = Guid.NewGuid(),
                SessionId = activeSession.Id,
                UserName = userName,
                StartTime = DateTime.UtcNow,
                EndTime = null,
                RoundNumber = activeSession.Rounds.Count + 1
            };

            await _uow.BeginTransactionAsync();
            try
            {
                await _uow.Rounds.CreateAsync(newRound);
                await _uow.CommitAsync();

                // Mapeo manual a DTO
                return new RoundDTO
                {
                    Id = newRound.Id,
                    UserName = newRound.UserName,
                    RoundNumber = newRound.RoundNumber,
                    StartTime = newRound.StartTime,
                    EndTime = newRound.EndTime,
                    SessionId = newRound.SessionId
                };
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }

        public async Task<RoundCloseResultDTO> CloseRoundAsync(Guid roundId)
        {
            await using var transaction = await _uow.BeginTransactionAsync();
            try
            {
                int resultNumber = _random.Next(0, 37);

                var round = await _uow.Rounds.GetByIdAsync(roundId);
                if (round == null || round.EndTime.HasValue)
                    throw new InvalidOperationException("Ronda no válida o ya cerrada");

                var bets = await _uow.Bets.GetBetsByRoundAsync(roundId);
                var betDtos = new List<BetDTO>();

                foreach (var bet in bets)
                {
                    var result = _betCalculator.CalculateResult(bet, resultNumber);
                    bet.Outcome = result.Outcome;
                    bet.Prize = result.Prize;

                    if (result.Outcome == BetOutcome.Win)
                    {
                        var user = await _uow.Users.GetByIdAsync(bet.UserId);
                        user.Balance += bet.Prize;
                        await _uow.Users.UpdateAsync(user);
                    }

                    await _uow.Bets.UpdateBetAsync(bet);

                    betDtos.Add(new BetDTO
                    {
                        Id = bet.Id,
                        UserName = bet.User?.UserName ?? bet.UserName,
                        BetType = bet.Type.ToString(),
                        BetValue = bet.BetValue,
                        Amount = bet.Amount,
                        TimeStamp = bet.TimeStamp,
                        Outcome = bet.Outcome.ToString(),
                        Prize = bet.Prize
                    });
                }

                round.EndTime = DateTime.UtcNow;
                round.Result = new SpinResultDTO
                {
                    ResultNumber = resultNumber,
                    Color = CalculateColor(resultNumber),
                    Parity = CalculateParity(resultNumber)
                };

                await _uow.Rounds.UpdateAsync(round);
                await _uow.SaveChangesAsync();
                await transaction.CommitAsync();

                return new RoundCloseResultDTO
                {
                    ResultNumber = resultNumber,
                    Color = round.Result.Color,
                    Parity = round.Result.Parity,
                    Bets = betDtos
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        private string CalculateColor(int number)
        {
            return number == 0 ? "Green" : (number % 2 == 0 ? "Black" : "Red");
        }

        private string CalculateParity(int number)
        {
            return number == 0 ? "None" : (number % 2 == 0 ? "Even" : "Odd");
        }
    

        public async Task<Round> GetRoundDetailsAsync(Guid roundId)
        {
            return await _uow.Rounds.GetByIdAsync(roundId);
        }
    }
}
