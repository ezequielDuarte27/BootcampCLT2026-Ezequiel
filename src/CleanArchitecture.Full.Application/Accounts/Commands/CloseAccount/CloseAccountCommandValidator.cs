using FluentValidation;

namespace CleanArchitecture.Full.Application.Accounts.Commands.CloseAccount;

public class CloseAccountCommandValidator : AbstractValidator<CloseAccountCommand>
{
    public CloseAccountCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
