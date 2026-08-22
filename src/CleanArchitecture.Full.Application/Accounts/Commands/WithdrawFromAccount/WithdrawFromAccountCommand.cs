using MediatR;

namespace CleanArchitecture.Full.Application.Accounts.Commands.WithdrawFromAccount;

public record WithdrawFromAccountCommand(Guid Id, decimal Amount) : IRequest<AccountDto?>;
