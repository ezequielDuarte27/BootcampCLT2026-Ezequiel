using MediatR;

namespace CleanArchitecture.Full.Application.Accounts.Commands.UpdateAccount;

public record UpdateAccountCommand(Guid Id, string Currency) : IRequest<AccountDto?>;
