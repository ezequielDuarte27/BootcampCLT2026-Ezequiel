using CleanArchitecture.Full.Application.Accounts.Commands.CreateAccount;
using FluentValidation.TestHelper;
using Xunit;

namespace CleanArchitecture.Full.Application.Tests.Accounts.Commands.CreateAccount;

public class CreateAccountCommandValidatorTests
{
    private readonly CreateAccountCommandValidator _validator = new();

    [Fact]
    public void Should_have_error_when_balance_is_negative()
    {
        var command = new CreateAccountCommand(Guid.NewGuid(), -1, "PYG");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Balance);
    }

    [Fact]
    public void Should_have_error_when_customer_id_is_empty()
    {
        var command = new CreateAccountCommand(Guid.Empty, 1000, "PYG");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CustomerId);
    }

    [Theory]
    [InlineData("PYG")]
    [InlineData("ARS")]
    [InlineData("USD")]
    [InlineData("EUR")]
    public void Should_not_have_error_for_allowed_currencies(string currency)
    {
        var command = new CreateAccountCommand(Guid.NewGuid(), 1000, currency);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Currency);
    }

    [Fact]
    public void Should_have_error_for_unsupported_currency()
    {
        var command = new CreateAccountCommand(Guid.NewGuid(), 1000, "BTC");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Currency);
    }
}
