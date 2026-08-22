using CleanArchitecture.Full.Application.Common;
using CleanArchitecture.Full.Application.Common.Exceptions;
using CleanArchitecture.Full.Domain;
using MediatR;

namespace CleanArchitecture.Full.Application.Accounts.Queries.GetAccountById;

public class GetAccountByIdQueryHandler(IAccountRepository repository, ICurrentUser currentUser)
    : IRequestHandler<GetAccountByIdQuery, AccountDto?>
{
    public async Task<AccountDto?> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken)
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

        return account.ToDto();
    }
}
