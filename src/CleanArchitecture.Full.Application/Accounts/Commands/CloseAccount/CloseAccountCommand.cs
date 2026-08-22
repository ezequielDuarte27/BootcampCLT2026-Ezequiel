using MediatR;

namespace CleanArchitecture.Full.Application.Accounts.Commands.CloseAccount;

public record CloseAccountCommand(Guid Id) : IRequest<bool>;
