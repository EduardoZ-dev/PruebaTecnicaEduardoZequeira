using RouletteTechTest.API.Data.Common;
using RouletteTechTest.API.Models.DTOs.Bet;
using RouletteTechTest.API.Models.Entities;
using RouletteTechTest.API.Models.Enums;
using RouletteTechTest.API.Services.Interfaces;
public class BetService : IBetService
{
    private readonly IUnitOfWork _uow;
    private readonly IBetCalculator _betCalculator;

    public BetService(IUnitOfWork uow, IBetCalculator betCalculator)
    {
        _uow = uow;
        _betCalculator = betCalculator;
    }

    public async Task<List<Bet>> GetBetsByRoundAsync(Guid roundId)
    {
        return await _uow.Bets.GetBetsByRoundAsync(roundId);
    }

    public async Task<Bet> GetBetByIdAsync(Guid betId)
    {
        var bet = await _uow.Bets.GetBetByIdAsync(betId);
        if (bet == null)
            throw new KeyNotFoundException("Apuesta no encontrada.");
        return bet;
    }

    public async Task UpdateBetAsync(Bet bet)
    {
        await _uow.Bets.UpdateBetAsync(bet);
    }

    public async Task<BetResultDTO> ProcessBetAndAdjustBalanceAsync(BetRequestDTO request)
    {
        await using var transaction = await _uow.BeginTransactionAsync();
        try
        {
            var user = await _uow.Users.GetByNameAsync(request.UserName);
            if (user == null)
                throw new KeyNotFoundException("Usuario no encontrado.");

            var round = await _uow.Rounds.GetByIdAsync(request.RoundId);
            if (round != null && round.EndTime.HasValue)
                throw new InvalidOperationException("La ronda ya está cerrada.");

            if (round == null)
            {
                var session = await _uow.Sessions.GetActiveSessionByUserAsync(user.UserName);

                if (session == null)
                {
                    session = new Session
                    {
                        Id = Guid.NewGuid(),
                        StartTime = DateTime.UtcNow,
                        Players = new List<User> { user }
                    };

                    await _uow.Sessions.AddAsync(session);
                    await _uow.SaveChangesAsync();
                }

                round = new Round
                {
                    Id = request.RoundId,
                    StartTime = DateTime.UtcNow,
                    SessionId = session.Id 
                };

                await _uow.Rounds.CreateAsync(round);
                await _uow.SaveChangesAsync();
            }

            ValidateBet(user, round, request.Amount);

            var bet = new Bet
            {
                UserId = user.Id,
                UserName = user.UserName,
                RoundId = round.Id,
                Type = request.Type,
                BetValue = request.BetValue,
                Amount = request.Amount,
                TimeStamp = DateTime.UtcNow,
                Outcome = BetOutcome.Pending
            };

            user.Balance -= request.Amount;
            await _uow.Users.UpdateAsync(user);
            await _uow.Bets.AddBetAsync(bet);

            await _uow.SaveChangesAsync();
            await transaction.CommitAsync();

            return new BetResultDTO
            {
                //WinningNumber = winningNumber,
                //Outcome = result.Outcome.ToString(),
                //Prize = result.Prize,
                Balance = user.Balance,
                BetAmount = bet.Amount,
                BetType = bet.Type.ToString(),
                BetValue = bet.BetValue
            };
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private void ValidateBet(User user, Round round, decimal amount)
    {
        if (round.EndTime.HasValue)
            throw new InvalidOperationException("No se pueden realizar apuestas en una ronda cerrada");

        if (user.Balance < amount)
            throw new InvalidOperationException("Saldo insuficiente");

        if (amount < 5 || amount > 10000)
            throw new ArgumentException("El monto debe estar entre 5 y 10,000");
    }

}