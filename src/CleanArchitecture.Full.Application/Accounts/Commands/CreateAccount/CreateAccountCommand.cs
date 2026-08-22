using MediatR;

namespace CleanArchitecture.Full.Application.Accounts.Commands.CreateAccount;

public record CreateAccountCommand(Guid CustomerId, decimal Balance, string Currency) : IRequest<AccountDto>;
