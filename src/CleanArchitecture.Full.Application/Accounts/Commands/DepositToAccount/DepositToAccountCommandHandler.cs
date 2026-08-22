using CleanArchitecture.Full.Application.Common;
using CleanArchitecture.Full.Application.Common.Exceptions;
using CleanArchitecture.Full.Domain;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Full.Application.Accounts.Commands.DepositToAccount;

public class DepositToAccountCommandHandler(
    IAccountRepository accountRepository,
    ITransactionRepository transactionRepository,
    ICurrentUser currentUser,
    ILogger<DepositToAccountCommandHandler> logger)
    : IRequestHandler<DepositToAccountCommand, AccountDto?>
{
    public async Task<AccountDto?> Handle(DepositToAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await accountRepository.GetByIdAsync(request.Id, cancellationToken);
        if (account is null)
        {
            return null;
        }

        if (!currentUser.IsAdmin && account.CustomerId != currentUser.CustomerId)
        {
            throw new ForbiddenAccessException();
        }

        if (account.Status != "Active")
        {
            throw new ValidationException([
                new ValidationFailure(nameof(Account.Status), "Solo se puede depositar en cuentas con estado 'Active'.")
            ]);
        }

        account.Balance += request.Amount;
        accountRepository.Update(account);

        await transactionRepository.AddAsync(new Transaction
        {
            Id = Guid.NewGuid(),
            AccountId = account.Id,
            Type = TransactionTypes.Deposit,
            Amount = request.Amount,
            Currency = account.Currency,
            BalanceAfter = account.Balance,
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);

        await accountRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Depósito de {Amount} realizado en la cuenta {AccountId} ({AccountNumber}). Nuevo balance: {Balance}",
            request.Amount,
            account.Id,
            account.AccountNumber,
            account.Balance);

        return account.ToDto();
    }
}
