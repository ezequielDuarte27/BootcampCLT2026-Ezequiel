using FluentValidation;

namespace CleanArchitecture.Full.Application.Accounts.Commands.ActivateAccount;

public class ActivateAccountCommandValidator : AbstractValidator<ActivateAccountCommand>
{
    public ActivateAccountCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
