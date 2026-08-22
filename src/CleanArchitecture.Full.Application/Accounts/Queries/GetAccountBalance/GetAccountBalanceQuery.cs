using MediatR;

namespace CleanArchitecture.Full.Application.Accounts.Queries.GetAccountBalance;

public record AccountBalanceDto(Guid AccountId, string AccountNumber, decimal Balance, string Currency);

public record GetAccountBalanceQuery(Guid Id) : IRequest<AccountBalanceDto?>;
