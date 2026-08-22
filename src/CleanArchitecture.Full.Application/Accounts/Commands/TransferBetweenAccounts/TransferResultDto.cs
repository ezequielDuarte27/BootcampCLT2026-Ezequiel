namespace CleanArchitecture.Full.Application.Accounts.Commands.TransferBetweenAccounts;

public record TransferResultDto(AccountDto Sender, AccountDto Beneficiary);
