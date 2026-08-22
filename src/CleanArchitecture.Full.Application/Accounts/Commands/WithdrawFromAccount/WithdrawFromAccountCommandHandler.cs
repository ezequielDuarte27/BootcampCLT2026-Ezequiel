using CleanArchitecture.Full.Application.Common;
using CleanArchitecture.Full.Application.Common.Exceptions;
using CleanArchitecture.Full.Domain;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Full.Application.Accounts.Commands.WithdrawFromAccount;

public class WithdrawFromAccountCommandHandler(
    IAccountRepository accountRepository,
    ITransactionRepository transactionRepository,
    ICurrentUser currentUser,
    ILogger<WithdrawFromAccountCommandHandler> logger)
    : IRequestHandler<WithdrawFromAccountCommand, AccountDto?>
{
    public async Task<AccountDto?> Handle(WithdrawFromAccountCommand request, CancellationToken cancellationToken)
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
        accountRepository.Update(account);

        await transactionRepository.AddAsync(new Transaction
        {
            Id = Guid.NewGuid(),
            AccountId = account.Id,
            Type = TransactionTypes.Withdrawal,
            Amount = request.Amount,
            Currency = account.Currency,
            BalanceAfter = account.Balance,
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);

        await accountRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Retiro de {Amount} realizado en la cuenta {AccountId} ({AccountNumber}). Nuevo balance: {Balance}",
            request.Amount,
            account.Id,
            account.AccountNumber,
            account.Balance);

        return account.ToDto();
    }
}
