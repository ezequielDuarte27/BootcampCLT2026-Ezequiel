using MediatR;

namespace CleanArchitecture.Full.Application.Accounts.Commands.TransferBetweenAccounts;

public record TransferBetweenAccountsCommand(Guid FromAccountId, Guid ToAccountId, decimal Amount) : IRequest<TransferResultDto?>;
