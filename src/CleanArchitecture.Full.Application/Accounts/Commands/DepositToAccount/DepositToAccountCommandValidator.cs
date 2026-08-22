using FluentValidation;

namespace CleanArchitecture.Full.Application.Accounts.Commands.DepositToAccount;

public class DepositToAccountCommandValidator : AbstractValidator<DepositToAccountCommand>
{
    public DepositToAccountCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("El monto a depositar debe ser mayor a 0.");
    }
}
