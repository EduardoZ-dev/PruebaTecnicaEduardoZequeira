using FluentValidation;
using RouletteTechTest.API.Models.Entities;
using RouletteTechTest.API.Models.Enums;

namespace RouletteTechTest.API.Models.Validations
{
    public class BetValidator : AbstractValidator<Bet>
    {
        public BetValidator()
        {
            // Regla general: El color debe ser "rojo" o "negro" si está presente
            RuleFor(b => b.BetColor)
                .Must(c => c == "rojo" || c == "negro")
                .When(b => !string.IsNullOrEmpty(b.BetColor))
                .WithMessage("El color debe ser 'rojo' o 'negro'");

            // Validaciones condicionales basadas en BetType
            RuleFor(b => b)
                .Must(b =>
                    (b.Type == BetType.Color || b.Type == BetType.Number)
                    && !string.IsNullOrEmpty(b.BetColor))
                .WithMessage("El color es obligatorio para apuestas de Color o Número")
                .OverridePropertyName(nameof(Bet.BetColor));

            RuleFor(b => b)
                .Must(b =>
                    b.Type == BetType.Number
                    && b.BetNumber.HasValue
                    && b.BetNumber >= 1
                    && b.BetNumber <= 36)
                .WithMessage("El número es obligatorio y debe estar entre 1 y 36 para apuestas de Número")
                .OverridePropertyName(nameof(Bet.BetNumber));

            RuleFor(b => b)
                .Must(b =>
                    b.Type == BetType.Parity
                    && (b.BetParity == "par" || b.BetParity == "impar"))
                .WithMessage("La paridad es obligatoria y debe ser 'par' o 'impar'")
                .OverridePropertyName(nameof(Bet.BetParity));
        }
    }
}
