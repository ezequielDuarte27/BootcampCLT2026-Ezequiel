using FluentValidation;

namespace CleanArchitecture.Full.Application.Accounts.Queries.GetAccountTransactions;

public class GetAccountTransactionsQueryValidator : AbstractValidator<GetAccountTransactionsQuery>
{
    public GetAccountTransactionsQueryValidator()
    {
        RuleFor(x => x.AccountId).NotEmpty();
    }
}
