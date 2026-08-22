using CleanArchitecture.Full.Application.Common;
using CleanArchitecture.Full.Domain;
using MediatR;

namespace CleanArchitecture.Full.Application.Accounts.Queries.GetAllAccounts;

public class GetAllAccountsQueryHandler(IAccountRepository repository, ICurrentUser currentUser)
    : IRequestHandler<GetAllAccountsQuery, IReadOnlyList<AccountDto>>
{
    public async Task<IReadOnlyList<AccountDto>> Handle(GetAllAccountsQuery request, CancellationToken cancellationToken)
    {
        var accounts = currentUser.IsAdmin
            ? await repository.GetAllAsync(cancellationToken)
            : await repository.GetAllByCustomerIdAsync(currentUser.CustomerId ?? Guid.Empty, cancellationToken);

        return accounts.Select(a => a.ToDto()).ToList();
    }
}
