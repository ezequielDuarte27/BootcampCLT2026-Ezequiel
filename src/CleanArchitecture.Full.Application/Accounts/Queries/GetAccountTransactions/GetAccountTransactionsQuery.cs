using MediatR;

namespace CleanArchitecture.Full.Application.Accounts.Queries.GetAccountTransactions;

public record TransactionDto(
    Guid Id,
    string Type,
    decimal Amount,
    string Currency,
    decimal BalanceAfter,
    Guid? RelatedAccountId,
    string? Description,
    DateTime CreatedAt);

public record GetAccountTransactionsQuery(Guid AccountId) : IRequest<IReadOnlyList<TransactionDto>?>;
