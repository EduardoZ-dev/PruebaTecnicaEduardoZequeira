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

        public async Task<Round> StartRoundAsync(Guid sessionId)
        {
            // Validar que la sesión existe
            var session = await _uow.Sessions.GetByIdAsync(sessionId);
            if (session == null)
                throw new InvalidOperationException("Sesión no encontrada");

            // Verificar si ya hay una ronda activa (sin cerrar) en esta sesión
            var activeRounds = await _uow.Rounds.GetRoundsBySessionIdAsync(sessionId);
            if (activeRounds.Any(r => !r.EndTime.HasValue))
                throw new InvalidOperationException("No puedes crear una nueva ronda hasta cerrar la anterior.");


            // Crear nueva ronda
            var newRound = new Round
            {
                SessionId = sessionId,
                StartTime = DateTime.UtcNow
            };

            // Guardar en la base de datos
            await _uow.Rounds.AddRoundAsync(newRound);
            await _uow.SaveChangesAsync();

            return newRound;
        }

        public async Task CloseRoundAsync(Guid roundId)
        {
            await using var transaction = await _uow.BeginTransactionAsync();
            try
            {
                int resultNumber = _random.Next(0, 37); // 0-36 inclusive

                // 1. Obtener la ronda
                var round = await _uow.Rounds.GetByIdAsync(roundId);
                if (round == null || round.EndTime.HasValue)
                    throw new InvalidOperationException("Ronda no válida o ya cerrada");

                // 2. Obtener apuestas de la ronda
                var bets = await _uow.Bets.GetBetsByRoundAsync(roundId);

                // 3. Procesar cada apuesta
                foreach (var bet in bets)
                {
                    var result = _betCalculator.CalculateResult(bet, resultNumber);
                    bet.Outcome = result.Outcome;
                    bet.Prize = result.Prize;

                    // Actualizar saldo si gana
                    if (result.Outcome == BetOutcome.Win)
                    {
                        var user = await _uow.Users.GetByIdAsync(bet.UserId);
                        user.Balance += bet.Prize;
                        await _uow.Users.UpdateAsync(user); 
                    }

                    await _uow.Bets.UpdateBetAsync(bet);
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

        public async Task<IEnumerable<Round>> GetRoundsBySessionAsync(Guid sessionId)
        {
            return await _uow.Rounds.GetRoundsBySessionIdAsync(sessionId);
        }
    }
}
