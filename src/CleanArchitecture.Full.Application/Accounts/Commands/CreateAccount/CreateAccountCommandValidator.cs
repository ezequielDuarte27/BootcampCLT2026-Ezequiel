using FluentValidation;

namespace CleanArchitecture.Full.Application.Accounts.Commands.CreateAccount;

public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
{
    private static readonly string[] AllowedCurrencies = ["PYG", "ARS", "USD", "EUR"];

    public CreateAccountCommandValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.Balance).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Currency).NotEmpty().Must(c => AllowedCurrencies.Contains(c))
            .WithMessage($"Currency debe ser una de: {string.Join(", ", AllowedCurrencies)}.");
    }
}
