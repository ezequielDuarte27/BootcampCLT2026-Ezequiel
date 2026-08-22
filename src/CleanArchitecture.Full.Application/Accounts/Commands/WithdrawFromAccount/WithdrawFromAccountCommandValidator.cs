using FluentValidation;

namespace CleanArchitecture.Full.Application.Accounts.Commands.WithdrawFromAccount;

public class WithdrawFromAccountCommandValidator : AbstractValidator<WithdrawFromAccountCommand>
{
    public WithdrawFromAccountCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("El monto a retirar debe ser mayor a 0.");
    }
}
