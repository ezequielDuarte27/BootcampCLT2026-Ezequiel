using MediatR;

namespace CleanArchitecture.Full.Application.Customers.Queries.GetCustomerById;

public record GetCustomerByIdQuery(Guid Id) : IRequest<CustomerDto?>;
