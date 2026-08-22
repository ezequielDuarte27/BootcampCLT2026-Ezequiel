using MediatR;

namespace CleanArchitecture.Full.Application.Accounts.Commands.DeactivateAccount;

public record DeactivateAccountCommand(Guid Id) : IRequest<AccountDto?>;
