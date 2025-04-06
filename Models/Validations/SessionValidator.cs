using FluentValidation;
using RouletteTechTest.API.Models.Entities;

namespace RouletteTechTest.API.Models.Validations
{
    public class SessionValidator : AbstractValidator<Session>
    {
        public SessionValidator()
        {
            // Validación para StartTime (no puede ser futura)
            RuleFor(x => x.StartTime)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("StartTime cannot be in the future");

            // Validación para EndTime (si existe, debe ser posterior a StartTime)
            RuleFor(x => x.EndTime)
                .Must((session, endTime) =>
                    !endTime.HasValue || endTime > session.StartTime)
                .WithMessage("EndTime must be after StartTime");

            // Validación para Players (mínimo 1 jugador)
            RuleFor(x => x.Players)
                .NotEmpty().WithMessage("At least one player is required")
                .Must(players => players.Any()).WithMessage("Players list cannot be empty");

            // Validación para Rounds (mínimo 1 ronda)
            RuleFor(x => x.Rounds)
                .NotEmpty().WithMessage("At least one round is required")
                .Must(rounds => rounds.Any()).WithMessage("Rounds list cannot be empty");
        }
    }
}
