using CleanArchitecture.Full.Application.Accounts.Commands.WithdrawFromAccount;
using CleanArchitecture.Full.Application.Tests.TestDoubles;
using CleanArchitecture.Full.Domain;
using FluentValidation;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace CleanArchitecture.Full.Application.Tests.Accounts.Commands.WithdrawFromAccount;

public class WithdrawFromAccountCommandHandlerTests
{
    private static Account CreateActiveAccount(decimal balance)
    {
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            DocumentType = "DNI",
            DocumentNumber = "12345678",
            FullName = "Test Customer",
            CreatedAt = DateTime.UtcNow
        };

        return new Account
        {
            Id = Guid.NewGuid(),
            AccountNumber = "ACC-000099",
            CustomerId = customer.Id,
            Customer = customer,
            Balance = balance,
            Currency = "PYG",
            Status = "Active",
            CreatedAt = DateTime.UtcNow
        };
    }

    [Fact]
    public async Task Should_throw_validation_exception_when_funds_are_insufficient()
    {
        var accountRepository = new InMemoryAccountRepository();
        var account = CreateActiveAccount(balance: 100m);
        accountRepository.Seed(account);

        var handler = new WithdrawFromAccountCommandHandler(
            accountRepository,
            new InMemoryTransactionRepository(),
            new TestCurrentUser(),
            NullLogger<WithdrawFromAccountCommandHandler>.Instance);

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.Handle(new WithdrawFromAccountCommand(account.Id, 500m), CancellationToken.None));
    }

    [Fact]
    public async Task Should_reduce_balance_and_record_transaction_on_successful_withdrawal()
    {
        var accountRepository = new InMemoryAccountRepository();
        var account = CreateActiveAccount(balance: 1000m);
        accountRepository.Seed(account);
        var transactionRepository = new InMemoryTransactionRepository();

        var handler = new WithdrawFromAccountCommandHandler(
            accountRepository,
            transactionRepository,
            new TestCurrentUser(),
            NullLogger<WithdrawFromAccountCommandHandler>.Instance);

        var result = await handler.Handle(new WithdrawFromAccountCommand(account.Id, 300m), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(700m, result.Balance);
        Assert.Single(transactionRepository.Transactions);
        Assert.Equal(TransactionTypes.Withdrawal, transactionRepository.Transactions[0].Type);
    }
}
