using CleanArchitecture.Full.Domain;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Full.Application.Accounts.Commands.WithdrawFromAccount;

public class WithdrawFromAccountCommandHandler(IAccountRepository repository, ILogger<WithdrawFromAccountCommandHandler> logger)
    : IRequestHandler<WithdrawFromAccountCommand, AccountDto?>
{
    public async Task<AccountDto?> Handle(WithdrawFromAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (account is null)
        {
            return null;
        }

        if (account.Status != "Active")
        {
            throw new ValidationException([
                new ValidationFailure(nameof(Account.Status), "Solo se puede retirar de cuentas con estado 'Active'.")
            ]);
        }

        if (account.Balance < request.Amount)
        {
            throw new ValidationException([
                new ValidationFailure(nameof(Account.Balance), "Fondos insuficientes para realizar el retiro.")
            ]);
        }

        account.Balance -= request.Amount;
        repository.Update(account);
        await repository.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Retiro de {Amount} realizado en la cuenta {AccountId} ({AccountNumber}). Nuevo balance: {Balance}",
            request.Amount,
            account.Id,
            account.AccountNumber,
            account.Balance);

        return account.ToDto();
    }
}
