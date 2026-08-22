using CleanArchitecture.Full.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Full.Application.Customers.Commands.CreateCustomer;

public class CreateCustomerCommandHandler(ICustomerRepository repository, ILogger<CreateCustomerCommandHandler> logger)
    : IRequestHandler<CreateCustomerCommand, CustomerDto>
{
    public async Task<CustomerDto> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            DocumentType = request.DocumentType,
            DocumentNumber = request.DocumentNumber,
            FullName = request.FullName,
            CreatedAt = DateTime.UtcNow
        };

        await repository.AddAsync(customer, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Cliente {CustomerId} ({DocumentType} {DocumentNumber}) creado",
            customer.Id,
            customer.DocumentType,
            customer.DocumentNumber);

        return customer.ToDto();
    }
}
