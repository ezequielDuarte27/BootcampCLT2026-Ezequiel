using CleanArchitecture.Full.Domain;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Full.Application.Accounts.Commands.TransferBetweenAccounts;

public class TransferBetweenAccountsCommandHandler(IAccountRepository repository, ILogger<TransferBetweenAccountsCommandHandler> logger)
    : IRequestHandler<TransferBetweenAccountsCommand, TransferResultDto?>
{
    public async Task<TransferResultDto?> Handle(TransferBetweenAccountsCommand request, CancellationToken cancellationToken)
    {
        var fromAccount = await repository.GetByIdAsync(request.FromAccountId, cancellationToken);
        if (fromAccount is null)
        {
            return null;
        }

        var toAccount = await repository.GetByIdAsync(request.ToAccountId, cancellationToken);
        if (toAccount is null)
        {
            return null;
        }

        if (fromAccount.Status != "Active" || toAccount.Status != "Active")
        {
            throw new ValidationException([
                new ValidationFailure(nameof(Account.Status), "Ambas cuentas deben estar en estado 'Active' para transferir.")
            ]);
        }

        if (fromAccount.Balance < request.Amount)
        {
            throw new ValidationException([
                new ValidationFailure(nameof(Account.Balance), "Fondos insuficientes en la cuenta de origen para realizar la transferencia.")
            ]);
        }

        fromAccount.Balance -= request.Amount;
        toAccount.Balance += request.Amount;

        repository.Update(fromAccount);
        repository.Update(toAccount);
        await repository.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Transferencia de {Amount} realizada desde la cuenta {FromAccountId} ({FromAccountNumber}) hacia la cuenta {ToAccountId} ({ToAccountNumber})",
            request.Amount,
            fromAccount.Id,
            fromAccount.AccountNumber,
            toAccount.Id,
            toAccount.AccountNumber);

        return new TransferResultDto(fromAccount.ToDto(), toAccount.ToDto());
    }
}
