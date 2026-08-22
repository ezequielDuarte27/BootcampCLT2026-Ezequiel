using CleanArchitecture.Full.Domain;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Full.Application.Accounts.Commands.CreateAccount;

public class CreateAccountCommandHandler(
    IAccountRepository accountRepository,
    ICustomerRepository customerRepository,
    ILogger<CreateAccountCommandHandler> logger)
    : IRequestHandler<CreateAccountCommand, AccountDto>
{
    private const decimal MinimumRecommendedBalance = 100m;
    private const int MaxAccountNumberAttempts = 5;

    public async Task<AccountDto> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var customer = await customerRepository.GetByIdAsync(request.CustomerId, cancellationToken)
            ?? throw new ValidationException([
                new ValidationFailure(nameof(CreateAccountCommand.CustomerId), "El cliente indicado no existe.")
            ]);

        var accountNumber = await GenerateUniqueAccountNumberAsync(cancellationToken);

        var account = new Account
        {
            Id = Guid.NewGuid(),
            AccountNumber = accountNumber,
            CustomerId = customer.Id,
            Customer = customer,
            Balance = request.Balance,
            Currency = request.Currency,
            Status = "Active",
            CreatedAt = DateTime.UtcNow
        };

        await accountRepository.AddAsync(account, cancellationToken);
        await accountRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Cuenta {AccountId} ({AccountNumber}) creada para el cliente {CustomerId}",
            account.Id,
            account.AccountNumber,
            customer.Id);

        if (account.Balance < MinimumRecommendedBalance)
        {
            // Regla de negocio "blanda": no bloquea la creación, solo advierte.
            logger.LogWarning(
                "Cuenta {AccountNumber} creada con balance {Balance} por debajo del recomendado {MinimumBalance}",
                account.AccountNumber,
                account.Balance,
                MinimumRecommendedBalance);
        }

        return account.ToDto();
    }

    private async Task<string> GenerateUniqueAccountNumberAsync(CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < MaxAccountNumberAttempts; attempt++)
        {
            var candidate = $"ACC-{Random.Shared.Next(0, 1_000_000):D6}";
            if (!await accountRepository.AccountNumberExistsAsync(candidate, cancellationToken))
            {
                return candidate;
            }
        }

        throw new InvalidOperationException("No se pudo generar un numero de cuenta unico luego de varios intentos.");
    }
}
