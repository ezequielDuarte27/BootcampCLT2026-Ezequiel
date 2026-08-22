using CleanArchitecture.Full.Application.Common;
using CleanArchitecture.Full.Application.Common.Exceptions;
using CleanArchitecture.Full.Domain;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Full.Application.Accounts.Commands.TransferBetweenAccounts;

public class TransferBetweenAccountsCommandHandler(
    IAccountRepository accountRepository,
    ITransactionRepository transactionRepository,
    ICurrentUser currentUser,
    ILogger<TransferBetweenAccountsCommandHandler> logger)
    : IRequestHandler<TransferBetweenAccountsCommand, TransferResultDto?>
{
    public async Task<TransferResultDto?> Handle(TransferBetweenAccountsCommand request, CancellationToken cancellationToken)
    {
        var senderAccount = await accountRepository.GetByAccountNumberAsync(request.Sender.AccountNumber, cancellationToken);
        if (senderAccount is null)
        {
            return null;
        }

        var beneficiaryAccount = await accountRepository.GetByAccountNumberAsync(request.Beneficiary.AccountNumber, cancellationToken);
        if (beneficiaryAccount is null)
        {
            return null;
        }

        if (!currentUser.IsAdmin && senderAccount.CustomerId != currentUser.CustomerId)
        {
            throw new ForbiddenAccessException();
        }

        if (senderAccount.Customer!.DocumentNumber != request.Sender.DocumentNumber)
        {
            throw new ValidationException([
                new ValidationFailure("Sender.DocumentNumber", "El documento del ordenante no coincide con el titular de la cuenta de origen.")
            ]);
        }

        if (beneficiaryAccount.Customer!.DocumentType != request.Beneficiary.DocumentType ||
            beneficiaryAccount.Customer!.DocumentNumber != request.Beneficiary.DocumentNumber)
        {
            throw new ValidationException([
                new ValidationFailure("Beneficiary.DocumentNumber", "Los datos del beneficiario no coinciden con el titular de la cuenta de destino.")
            ]);
        }

        if (senderAccount.Id == beneficiaryAccount.Id)
        {
            throw new ValidationException([
                new ValidationFailure("Beneficiary.AccountNumber", "La cuenta de origen y la cuenta de destino no pueden ser la misma.")
            ]);
        }

        if (senderAccount.Status != "Active" || beneficiaryAccount.Status != "Active")
        {
            throw new ValidationException([
                new ValidationFailure(nameof(Account.Status), "Ambas cuentas deben estar en estado 'Active' para transferir.")
            ]);
        }

        if (senderAccount.Currency != request.Currency || beneficiaryAccount.Currency != request.Currency)
        {
            throw new ValidationException([
                new ValidationFailure(nameof(TransferBetweenAccountsCommand.Currency), "La moneda de la transferencia debe coincidir con la moneda de ambas cuentas.")
            ]);
        }

        if (senderAccount.Balance < request.Amount)
        {
            throw new ValidationException([
                new ValidationFailure(nameof(Account.Balance), "Fondos insuficientes en la cuenta de origen para realizar la transferencia.")
            ]);
        }

        senderAccount.Balance -= request.Amount;
        beneficiaryAccount.Balance += request.Amount;

        accountRepository.Update(senderAccount);
        accountRepository.Update(beneficiaryAccount);

        var now = DateTime.UtcNow;

        await transactionRepository.AddAsync(new Transaction
        {
            Id = Guid.NewGuid(),
            AccountId = senderAccount.Id,
            Type = TransactionTypes.TransferOut,
            Amount = request.Amount,
            Currency = request.Currency,
            BalanceAfter = senderAccount.Balance,
            RelatedAccountId = beneficiaryAccount.Id,
            CreatedAt = now
        }, cancellationToken);

        await transactionRepository.AddAsync(new Transaction
        {
            Id = Guid.NewGuid(),
            AccountId = beneficiaryAccount.Id,
            Type = TransactionTypes.TransferIn,
            Amount = request.Amount,
            Currency = request.Currency,
            BalanceAfter = beneficiaryAccount.Balance,
            RelatedAccountId = senderAccount.Id,
            CreatedAt = now
        }, cancellationToken);

        await accountRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Transferencia de {Amount} {Currency} realizada desde {SenderAccountNumber} hacia {BeneficiaryAccountNumber}",
            request.Amount,
            request.Currency,
            senderAccount.AccountNumber,
            beneficiaryAccount.AccountNumber);

        return new TransferResultDto(senderAccount.ToDto(), beneficiaryAccount.ToDto());
    }
}
