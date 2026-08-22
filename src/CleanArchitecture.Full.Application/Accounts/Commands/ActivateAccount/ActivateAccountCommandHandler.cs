using CleanArchitecture.Full.Domain;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Full.Application.Accounts.Commands.ActivateAccount;

public class ActivateAccountCommandHandler(IAccountRepository repository, ILogger<ActivateAccountCommandHandler> logger)
    : IRequestHandler<ActivateAccountCommand, AccountDto?>
{
    public async Task<AccountDto?> Handle(ActivateAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (account is null)
        {
            return null;
        }

        if (account.Status == "Closed")
        {
            throw new ValidationException([
                new ValidationFailure(nameof(Account.Status), "No se puede activar una cuenta cerrada.")
            ]);
        }

        account.Status = "Active";
        repository.Update(account);
        await repository.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Cuenta {AccountId} ({AccountNumber}) activada",
            account.Id,
            account.AccountNumber);

        return account.ToDto();
    }
}
