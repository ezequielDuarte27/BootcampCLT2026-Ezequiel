using CleanArchitecture.Full.Application.Common;
using CleanArchitecture.Full.Application.Common.Exceptions;
using CleanArchitecture.Full.Domain;
using MediatR;

namespace CleanArchitecture.Full.Application.Accounts.Queries.GetAccountTransactions;

public class GetAccountTransactionsQueryHandler(
    IAccountRepository accountRepository,
    ITransactionRepository transactionRepository,
    ICurrentUser currentUser)
    : IRequestHandler<GetAccountTransactionsQuery, IReadOnlyList<TransactionDto>?>
{
    public async Task<IReadOnlyList<TransactionDto>?> Handle(GetAccountTransactionsQuery request, CancellationToken cancellationToken)
    {
        var account = await accountRepository.GetByIdAsync(request.AccountId, cancellationToken);
        if (account is null)
        {
            return null;
        }

        if (!currentUser.IsAdmin && account.CustomerId != currentUser.CustomerId)
        {
            throw new ForbiddenAccessException();
        }

        var transactions = await transactionRepository.GetByAccountIdAsync(request.AccountId, cancellationToken);

        return transactions
            .Select(t => new TransactionDto(t.Id, t.Type, t.Amount, t.Currency, t.BalanceAfter, t.RelatedAccountId, t.Description, t.CreatedAt))
            .ToList();
    }
}
