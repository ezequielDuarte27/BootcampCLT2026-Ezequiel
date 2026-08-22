using FluentValidation;

namespace CleanArchitecture.Full.Application.Accounts.Queries.GetAccountBalance;

public class GetAccountBalanceQueryValidator : AbstractValidator<GetAccountBalanceQuery>
{
    public GetAccountBalanceQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
