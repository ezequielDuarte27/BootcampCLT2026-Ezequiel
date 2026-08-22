using CleanArchitecture.Full.Domain;

namespace CleanArchitecture.Full.Application.Customers;

public static class CustomerMappingExtensions
{
    public static CustomerDto ToDto(this Customer customer) =>
        new(customer.Id, customer.DocumentType, customer.DocumentNumber, customer.FullName, customer.CreatedAt);
}
