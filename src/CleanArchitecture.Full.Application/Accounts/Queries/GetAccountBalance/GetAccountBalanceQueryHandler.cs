using CleanArchitecture.Full.Application.Common;
using CleanArchitecture.Full.Application.Common.Exceptions;
using CleanArchitecture.Full.Domain;
using MediatR;

namespace CleanArchitecture.Full.Application.Accounts.Queries.GetAccountBalance;

public class GetAccountBalanceQueryHandler(IAccountRepository repository, ICurrentUser currentUser)
    : IRequestHandler<GetAccountBalanceQuery, AccountBalanceDto?>
{
    public async Task<AccountBalanceDto?> Handle(GetAccountBalanceQuery request, CancellationToken cancellationToken)
    {
        var account = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (account is null)
        {
            return null;
        }

        if (!currentUser.IsAdmin && account.CustomerId != currentUser.CustomerId)
        {
            throw new ForbiddenAccessException();
        }

        return new AccountBalanceDto(account.Id, account.AccountNumber, account.Balance, account.Currency);
    }
}
