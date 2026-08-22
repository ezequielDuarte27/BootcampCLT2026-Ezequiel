using CleanArchitecture.Full.Domain;

namespace CleanArchitecture.Full.Application.Accounts;

public static class AccountMappingExtensions
{
    public static AccountDto ToDto(this Account account)
    {
        if (account.Customer is null)
        {
            throw new InvalidOperationException("El cliente de la cuenta no fue cargado.");
        }

        return new AccountDto(
            account.Id,
            account.AccountNumber,
            new CustomerSummaryDto(account.Customer.Id, account.Customer.DocumentType, account.Customer.DocumentNumber, account.Customer.FullName),
            account.Balance,
            account.Currency,
            account.Status,
            account.CreatedAt,
            account.ClosedAt);
    }
}
