using FluentValidation;

namespace CleanArchitecture.Full.Application.Accounts.Commands.TransferBetweenAccounts;

public class TransferBetweenAccountsCommandValidator : AbstractValidator<TransferBetweenAccountsCommand>
{
    private static readonly string[] AllowedCurrencies = ["PYG", "ARS", "USD", "EUR"];

    public TransferBetweenAccountsCommandValidator()
    {
        RuleFor(x => x.Sender.AccountNumber).NotEmpty();
        RuleFor(x => x.Sender.DocumentNumber).NotEmpty();
        RuleFor(x => x.Beneficiary.AccountNumber).NotEmpty();
        RuleFor(x => x.Beneficiary.DocumentType).NotEmpty();
        RuleFor(x => x.Beneficiary.DocumentNumber).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("El monto a transferir debe ser mayor a 0.");
        RuleFor(x => x.Currency).NotEmpty().Must(c => AllowedCurrencies.Contains(c))
            .WithMessage($"Currency debe ser una de: {string.Join(", ", AllowedCurrencies)}.");
    }
}
