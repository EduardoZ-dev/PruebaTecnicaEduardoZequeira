using FluentValidation;
using RouletteTechTest.API.Models.DTOs.User;

namespace RouletteTechTest.API.Models.Validations
{
    public class UserValidator : AbstractValidator<UserCreateDTO>
    {
        public UserValidator()
        {
            RuleFor(user => user.UserName)
                .NotEmpty()
                .WithMessage("El nombre de usuario es obligatorio.");

            RuleFor(user => user.InitialBalance)
                .NotNull()
                .WithMessage("El saldo inicial es obligatorio.")
                .GreaterThanOrEqualTo(1)
                .WithMessage("El saldo inicial debe ser mayor o igual a 1.")
                .LessThanOrEqualTo(10000)
                .WithMessage("El saldo inicial debe ser menor o igual a 10000.");
        }
    }
}
