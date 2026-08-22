using FluentValidation;

namespace CleanArchitecture.Full.Application.Accounts.Commands.UpdateAccount;

public class UpdateAccountCommandValidator : AbstractValidator<UpdateAccountCommand>
{
    private static readonly string[] AllowedCurrencies = ["PYG", "ARS", "USD", "EUR"];

    public UpdateAccountCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Currency).NotEmpty().Must(c => AllowedCurrencies.Contains(c))
            .WithMessage($"Currency debe ser una de: {string.Join(", ", AllowedCurrencies)}.");
    }
}
