using MediatR;

namespace CleanArchitecture.Full.Application.Accounts.Commands.ActivateAccount;

public record ActivateAccountCommand(Guid Id) : IRequest<AccountDto?>;
