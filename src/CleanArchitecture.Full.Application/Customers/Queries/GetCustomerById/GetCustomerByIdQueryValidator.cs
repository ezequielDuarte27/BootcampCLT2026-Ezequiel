using FluentValidation;

namespace CleanArchitecture.Full.Application.Customers.Queries.GetCustomerById;

public class GetCustomerByIdQueryValidator : AbstractValidator<GetCustomerByIdQuery>
{
    public GetCustomerByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
