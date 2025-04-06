using FluentValidation;
using RouletteTechTest.API.Models.Entities;

namespace RouletteTechTest.API.Models.Validations
{
    public class RoundValidator : AbstractValidator<Round>
    {
        public RoundValidator()
        {
            // Validación para RoundNumber (mínimo 1)
            RuleFor(x => x.RoundNumber)
                .GreaterThanOrEqualTo(1)
                .WithMessage("RoundNumber must be at least 1");

            // Validación para StartTime (no puede ser futura)
            RuleFor(x => x.StartTime)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("StartTime cannot be in the future");

            // Validación para EndTime (si existe, debe ser posterior a StartTime)
            RuleFor(x => x.EndTime)
                .Must((round, endTime) =>
                    !endTime.HasValue || endTime > round.StartTime)
                .WithMessage("EndTime must be after StartTime");

            // Validación para Bets (mínimo 1 apuesta)
            RuleFor(x => x.Bets)
                .NotEmpty().WithMessage("At least one bet is required")
                .Must(bets => bets.Any()).WithMessage("Bets list cannot be empty");

            // Validación condicional: Si hay EndTime, debe haber Result
            RuleFor(x => x.Result)
                .NotNull()
                .When(x => x.EndTime.HasValue)
                .WithMessage("Result is required when the round has ended");

            // Validación para SessionId (no puede ser vacío)
            RuleFor(x => x.SessionId)
                .NotEmpty().WithMessage("SessionId is required");
        }
    }
}
