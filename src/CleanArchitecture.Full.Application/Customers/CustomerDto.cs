namespace CleanArchitecture.Full.Application.Customers;

public record CustomerDto(Guid Id, string DocumentType, string DocumentNumber, string FullName, DateTime CreatedAt);
