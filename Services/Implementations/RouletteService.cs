using FluentValidation;
using RouletteTechTest.API.Models.DTOs.Bet;
using RouletteTechTest.API.Models.DTOs;
using RouletteTechTest.API.Models.Entities;
using RouletteTechTest.API.Services.Interfaces;
using RouletteTechTest.API.Models.Enums;

namespace RouletteTechTest.API.Services.Implementations
{
    public class RouletteService : IRouletteService
    {
        private readonly IUserRepository _userRepo;
        private readonly ISessionRepository _sessionRepo;
        private readonly IBetRepository _betRepo;
        private readonly IValidator<Bet> _betValidator;
        private readonly Random _random = new();

        public RouletteService(
            IUserRepository userRepo,
            ISessionRepository sessionRepo,
            IBetRepository betRepo,
            IValidator<Bet> betValidator)
        {
            _userRepo = userRepo;
            _sessionRepo = sessionRepo;
            _betRepo = betRepo;
            _betValidator = betValidator;
        }

        public async Task<SpinResultDTO> SpinAsync()
        {
            return new SpinResultDTO
            {
                Number = _random.Next(0, 37),
                Color = _random.Next(0, 2) == 0 ? "rojo" : "negro",
                IsEven = _random.Next(0, 37) % 2 == 0
            };
        }

        public async Task<BetResponseDTO> PlaceBetAsync(Guid userId, BetPlaceDTO betDTO)
        {
            // Validar usuario y saldo
            var user = await _userRepo.GetByIdAsync(userId)
                ?? throw new ArgumentException("Usuario no encontrado");

            if (user.Balance < betDTO.Amount)
                throw new InvalidOperationException("Saldo insuficiente");

            // Obtener/Crear sesión activa
            var session = await GetOrCreateActiveSession(userId, user.Balance);

            // Generar resultado del spin
            var spinResult = await SpinAsync();

            // Crear apuesta y validar
            var bet = new Bet
            {
                SessionId = session.Id,
                Amount = betDTO.Amount,
                Type = betDTO.Type,
                BetColor = betDTO.Color,
                BetNumber = betDTO.Number,
                BetParity = betDTO.Parity,
                TimeStamp = DateTime.UtcNow
            };

            var validationResult = await _betValidator.ValidateAsync(bet);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            // Calcular premio y resultado
            (bet.Outcome, bet.Prize) = CalculatePrize(bet, spinResult);

            // Actualizar saldo del usuario
            user.Balance += bet.Prize;
            await _userRepo.UpdateAsync(user);

            // Guardar apuesta
            await _betRepo.AddAsync(bet);

            return MapBetToDto(bet);
        }

        private async Task<Session> GetOrCreateActiveSession(Guid userId, decimal initialBalance)
        {
            var activeSession = (await _sessionRepo.GetByUserIdAsync(userId))
                .FirstOrDefault(s => s.EndTime == null);

            if (activeSession == null)
            {
                activeSession = new Session
                {
                    UserId = userId,
                    InitialBalance = initialBalance,
                    StartTime = DateTime.UtcNow
                };
                await _sessionRepo.AddAsync(activeSession);
            }

            return activeSession;
        }

        private (BetOutcome Outcome, decimal Prize) CalculatePrize(Bet bet, SpinResultDTO spin)
        {
            bool isWin = false;
            decimal prize = 0;

            switch (bet.Type)
            {
                case BetType.Color:
                    isWin = bet.BetColor?.ToLower() == spin.Color.ToLower();
                    prize = isWin ? bet.Amount * 0.5m : -bet.Amount;
                    break;

                case BetType.Parity:
                    bool isBetEven = bet.BetParity?.ToLower() == "par";
                    isWin = spin.IsEven == isBetEven;
                    prize = isWin ? bet.Amount : -bet.Amount;
                    break;

                case BetType.Number:
                    isWin = bet.BetNumber == spin.Number
                            && bet.BetColor?.ToLower() == spin.Color.ToLower();
                    prize = isWin ? bet.Amount * 3 : -bet.Amount;
                    break;
            }

            return (isWin ? BetOutcome.Win : BetOutcome.Lose, prize);
        }

        private BetResponseDTO MapBetToDto(Bet bet)
        {
            return new BetResponseDTO
            {
                Id = bet.Id,
                Amount = bet.Amount,
                Outcome = bet.Outcome,
                Prize = bet.Prize,
                BetType = bet.Type.ToString(),
                Timestamp = bet.TimeStamp
            };
        }
    }
}
