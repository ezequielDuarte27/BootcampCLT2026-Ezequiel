using CleanArchitecture.Full.Domain;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Full.Application.Accounts.Commands.DeactivateAccount;

public class DeactivateAccountCommandHandler(IAccountRepository repository, ILogger<DeactivateAccountCommandHandler> logger)
    : IRequestHandler<DeactivateAccountCommand, AccountDto?>
{
    public async Task<AccountDto?> Handle(DeactivateAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (account is null)
        {
            return null;
        }

        if (account.Status == "Closed")
        {
            throw new ValidationException([
                new ValidationFailure(nameof(Account.Status), "No se puede desactivar una cuenta cerrada.")
            ]);
        }

        account.Status = "Inactive";
        repository.Update(account);
        await repository.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Cuenta {AccountId} ({AccountNumber}) desactivada",
            account.Id,
            account.AccountNumber);

        return account.ToDto();
    }
}
