using CleanArchitecture.Full.Application.Accounts;
using CleanArchitecture.Full.Application.Accounts.Commands.ActivateAccount;
using CleanArchitecture.Full.Application.Accounts.Commands.CreateAccount;
using CleanArchitecture.Full.Application.Accounts.Commands.DeactivateAccount;
using CleanArchitecture.Full.Application.Accounts.Commands.DeleteAccount;
using CleanArchitecture.Full.Application.Accounts.Commands.DepositToAccount;
using CleanArchitecture.Full.Application.Accounts.Commands.TransferBetweenAccounts;
using CleanArchitecture.Full.Application.Accounts.Commands.UpdateAccount;
using CleanArchitecture.Full.Application.Accounts.Commands.WithdrawFromAccount;
using CleanArchitecture.Full.Application.Accounts.Queries.GetAllAccounts;
using CleanArchitecture.Full.Application.Accounts.Queries.GetAccountById;
using MediatR;

namespace CleanArchitecture.Full.Api.Endpoints;

public static class AccountEndpoints
{
    public static void MapAccountEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/v1/accounts").WithTags("Accounts");

        group.MapGet("", async (ISender sender, CancellationToken cancellationToken) =>
            Results.Ok(await sender.Send(new GetAllAccountsQuery(), cancellationToken)))
            .Produces<IReadOnlyList<AccountDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapGet("{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
        {
            var account = await sender.Send(new GetAccountByIdQuery(id), cancellationToken);
            return account is null ? Results.NotFound() : Results.Ok(account);
        })
            .Produces<AccountDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("", async (CreateAccountCommand command, ISender sender, CancellationToken cancellationToken) =>
        {
            var account = await sender.Send(command, cancellationToken);
            return Results.Created($"api/v1/accounts/{account.Id}", account);
        })
            .Produces<AccountDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPut("{id:guid}", async (Guid id, UpdateAccountBody body, ISender sender, CancellationToken cancellationToken) =>
        {
            var account = await sender.Send(new UpdateAccountCommand(id, body.AccountNumber, body.HolderName, body.Balance, body.Status), cancellationToken);
            return account is null ? Results.NotFound() : Results.Ok(account);
        })
            .Produces<AccountDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapDelete("{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
        {
            var deleted = await sender.Send(new DeleteAccountCommand(id), cancellationToken);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("{id:guid}/deposit", async (Guid id, AccountAmountBody body, ISender sender, CancellationToken cancellationToken) =>
        {
            var account = await sender.Send(new DepositToAccountCommand(id, body.Amount), cancellationToken);
            return account is null ? Results.NotFound() : Results.Ok(account);
        })
            .Produces<AccountDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("{id:guid}/withdraw", async (Guid id, AccountAmountBody body, ISender sender, CancellationToken cancellationToken) =>
        {
            var account = await sender.Send(new WithdrawFromAccountCommand(id, body.Amount), cancellationToken);
            return account is null ? Results.NotFound() : Results.Ok(account);
        })
            .Produces<AccountDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("{id:guid}/transfer", async (Guid id, TransferBody body, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new TransferBetweenAccountsCommand(id, body.ToAccountId, body.Amount), cancellationToken);
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
            .Produces<TransferResultDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("{id:guid}/activate", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
        {
            var account = await sender.Send(new ActivateAccountCommand(id), cancellationToken);
            return account is null ? Results.NotFound() : Results.Ok(account);
        })
            .Produces<AccountDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("{id:guid}/deactivate", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
        {
            var account = await sender.Send(new DeactivateAccountCommand(id), cancellationToken);
            return account is null ? Results.NotFound() : Results.Ok(account);
        })
            .Produces<AccountDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}

public record UpdateAccountBody(string AccountNumber, string HolderName, decimal Balance, string Status);

public record AccountAmountBody(decimal Amount);

public record TransferBody(Guid ToAccountId, decimal Amount);
