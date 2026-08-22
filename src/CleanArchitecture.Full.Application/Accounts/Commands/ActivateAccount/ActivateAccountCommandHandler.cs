using CleanArchitecture.Full.Domain;
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
