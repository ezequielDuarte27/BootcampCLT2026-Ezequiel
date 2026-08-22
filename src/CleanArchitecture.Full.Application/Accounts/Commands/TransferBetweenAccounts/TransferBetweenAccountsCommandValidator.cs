using FluentValidation;

namespace CleanArchitecture.Full.Application.Accounts.Commands.TransferBetweenAccounts;

public class TransferBetweenAccountsCommandValidator : AbstractValidator<TransferBetweenAccountsCommand>
{
    public TransferBetweenAccountsCommandValidator()
    {
        RuleFor(x => x.FromAccountId).NotEmpty();
        RuleFor(x => x.ToAccountId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("El monto a transferir debe ser mayor a 0.");
        RuleFor(x => x.ToAccountId)
            .NotEqual(x => x.FromAccountId)
            .WithMessage("La cuenta de origen y la cuenta de destino no pueden ser la misma.");
    }
}
