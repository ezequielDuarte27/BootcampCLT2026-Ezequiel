using FluentValidation;

namespace CleanArchitecture.Full.Application.Auth.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8)
            .WithMessage("La contraseña debe tener al menos 8 caracteres.");
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.DocumentNumber).NotEmpty();
    }
}
