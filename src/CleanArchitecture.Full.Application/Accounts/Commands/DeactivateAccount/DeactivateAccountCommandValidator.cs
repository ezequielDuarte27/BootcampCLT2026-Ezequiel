using FluentValidation;

namespace CleanArchitecture.Full.Application.Accounts.Commands.DeactivateAccount;

public class DeactivateAccountCommandValidator : AbstractValidator<DeactivateAccountCommand>
{
    public DeactivateAccountCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
