using MediatR;

namespace CleanArchitecture.Full.Application.Customers.Commands.CreateCustomer;

public record CreateCustomerCommand(string DocumentType, string DocumentNumber, string FullName) : IRequest<CustomerDto>;
