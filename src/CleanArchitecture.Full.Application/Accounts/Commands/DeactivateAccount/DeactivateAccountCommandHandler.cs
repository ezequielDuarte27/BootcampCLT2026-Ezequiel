using CleanArchitecture.Full.Domain;
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
