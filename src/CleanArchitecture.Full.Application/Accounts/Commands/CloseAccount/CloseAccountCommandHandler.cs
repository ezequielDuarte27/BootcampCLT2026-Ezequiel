using CleanArchitecture.Full.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Full.Application.Accounts.Commands.CloseAccount;

public class CloseAccountCommandHandler(IAccountRepository repository, ILogger<CloseAccountCommandHandler> logger)
    : IRequestHandler<CloseAccountCommand, bool>
{
    public async Task<bool> Handle(CloseAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (account is null)
        {
            return false;
        }

        if (account.Status == "Closed")
        {
            return true;
        }

        account.Status = "Closed";
        account.ClosedAt = DateTime.UtcNow;
        repository.Update(account);
        await repository.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Cuenta {AccountId} ({AccountNumber}) cerrada",
            account.Id,
            account.AccountNumber);

        return true;
    }
}
