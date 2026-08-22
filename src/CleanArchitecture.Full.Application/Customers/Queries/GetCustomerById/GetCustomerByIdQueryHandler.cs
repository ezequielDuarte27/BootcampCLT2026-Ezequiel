using CleanArchitecture.Full.Application.Common;
using CleanArchitecture.Full.Application.Common.Exceptions;
using CleanArchitecture.Full.Domain;
using MediatR;

namespace CleanArchitecture.Full.Application.Customers.Queries.GetCustomerById;

public class GetCustomerByIdQueryHandler(ICustomerRepository repository, ICurrentUser currentUser)
    : IRequestHandler<GetCustomerByIdQuery, CustomerDto?>
{
    public async Task<CustomerDto?> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (customer is null)
        {
            return null;
        }

        if (!currentUser.IsAdmin && currentUser.CustomerId != customer.Id)
        {
            throw new ForbiddenAccessException();
        }

        return customer.ToDto();
    }
}
