using CleanArchitecture.Full.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Full.Application.Accounts.Commands.UpdateAccount;

public class UpdateAccountCommandHandler(IAccountRepository repository, ILogger<UpdateAccountCommandHandler> logger)
    : IRequestHandler<UpdateAccountCommand, AccountDto?>
{
    public async Task<AccountDto?> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (account is null)
        {
            return null;
        }

        account.Currency = request.Currency;
        repository.Update(account);
        await repository.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Cuenta {AccountId} ({AccountNumber}) actualizada: moneda {Currency}",
            account.Id,
            account.AccountNumber,
            account.Currency);

        return account.ToDto();
    }
}
