using MediatR;

namespace CleanArchitecture.Full.Application.Accounts.Commands.DepositToAccount;

public record DepositToAccountCommand(Guid Id, decimal Amount) : IRequest<AccountDto?>;
