namespace CleanArchitecture.Full.Application.Accounts;

public record CustomerSummaryDto(Guid Id, string DocumentType, string DocumentNumber, string FullName);

public record AccountDto(
    Guid Id,
    string AccountNumber,
    CustomerSummaryDto Customer,
    decimal Balance,
    string Currency,
    string Status,
    DateTime CreatedAt,
    DateTime? ClosedAt);
