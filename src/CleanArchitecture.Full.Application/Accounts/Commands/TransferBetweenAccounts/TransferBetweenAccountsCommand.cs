using MediatR;

namespace CleanArchitecture.Full.Application.Accounts.Commands.TransferBetweenAccounts;

public record TransferSender(string AccountNumber, string DocumentNumber);

public record TransferBeneficiary(string AccountNumber, string DocumentType, string DocumentNumber);

public record TransferBetweenAccountsCommand(
    TransferSender Sender,
    TransferBeneficiary Beneficiary,
    decimal Amount,
    string Currency) : IRequest<TransferResultDto?>;
