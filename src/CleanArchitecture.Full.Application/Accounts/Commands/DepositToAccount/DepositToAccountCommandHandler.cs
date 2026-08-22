using CleanArchitecture.Full.Domain;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Full.Application.Accounts.Commands.DepositToAccount;

public class DepositToAccountCommandHandler(IAccountRepository repository, ILogger<DepositToAccountCommandHandler> logger)
    : IRequestHandler<DepositToAccountCommand, AccountDto?>
{
    public async Task<AccountDto?> Handle(DepositToAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (account is null)
        {
            return null;
        }

        if (account.Status != "Active")
        {
            throw new ValidationException([
                new ValidationFailure(nameof(Account.Status), "Solo se puede depositar en cuentas con estado 'Active'.")
            ]);
        }

        account.Balance += request.Amount;
        repository.Update(account);
        await repository.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Depósito de {Amount} realizado en la cuenta {AccountId} ({AccountNumber}). Nuevo balance: {Balance}",
            request.Amount,
            account.Id,
            account.AccountNumber,
            account.Balance);

        return account.ToDto();
    }
}
