using CleanArchitecture.Full.Application.Accounts;
using CleanArchitecture.Full.Application.Accounts.Commands.ActivateAccount;
using CleanArchitecture.Full.Application.Accounts.Commands.CloseAccount;
using CleanArchitecture.Full.Application.Accounts.Commands.CreateAccount;
using CleanArchitecture.Full.Application.Accounts.Commands.DeactivateAccount;
using CleanArchitecture.Full.Application.Accounts.Commands.DepositToAccount;
using CleanArchitecture.Full.Application.Accounts.Commands.UpdateAccount;
using CleanArchitecture.Full.Application.Accounts.Commands.WithdrawFromAccount;
using CleanArchitecture.Full.Application.Accounts.Queries.GetAccountBalance;
using CleanArchitecture.Full.Application.Accounts.Queries.GetAccountById;
using CleanArchitecture.Full.Application.Accounts.Queries.GetAccountTransactions;
using CleanArchitecture.Full.Application.Accounts.Queries.GetAllAccounts;
using MediatR;

namespace CleanArchitecture.Full.Api.Endpoints;

public static class AccountEndpoints
{
    public static void MapAccountEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/v1/accounts").WithTags("Accounts").RequireAuthorization();

        group.MapGet("", async (ISender sender, CancellationToken cancellationToken) =>
            Results.Ok(await sender.Send(new GetAllAccountsQuery(), cancellationToken)))
            .Produces<IReadOnlyList<AccountDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapGet("{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
        {
            var account = await sender.Send(new GetAccountByIdQuery(id), cancellationToken);
            return account is null ? Results.NotFound() : Results.Ok(account);
        })
            .Produces<AccountDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("{id:guid}/balance", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
        {
            var balance = await sender.Send(new GetAccountBalanceQuery(id), cancellationToken);
            return balance is null ? Results.NotFound() : Results.Ok(balance);
        })
            .Produces<AccountBalanceDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("{id:guid}/transactions", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
        {
            var transactions = await sender.Send(new GetAccountTransactionsQuery(id), cancellationToken);
            return transactions is null ? Results.NotFound() : Results.Ok(transactions);
        })
            .Produces<IReadOnlyList<TransactionDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("", async (CreateAccountBody body, ISender sender, CancellationToken cancellationToken) =>
        {
            var account = await sender.Send(new CreateAccountCommand(body.CustomerId, body.Balance, body.Currency), cancellationToken);
            return Results.Created($"api/v1/accounts/{account.Id}", account);
        })
            .RequireAuthorization("AdminOnly")
            .Produces<AccountDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);

        group.MapPut("{id:guid}", async (Guid id, UpdateAccountBody body, ISender sender, CancellationToken cancellationToken) =>
        {
            var account = await sender.Send(new UpdateAccountCommand(id, body.Currency), cancellationToken);
            return account is null ? Results.NotFound() : Results.Ok(account);
        })
            .RequireAuthorization("AdminOnly")
            .Produces<AccountDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
        {
            var closed = await sender.Send(new CloseAccountCommand(id), cancellationToken);
            return closed ? Results.NoContent() : Results.NotFound();
        })
            .RequireAuthorization("AdminOnly")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("{id:guid}/deposit", async (Guid id, AccountAmountBody body, ISender sender, CancellationToken cancellationToken) =>
        {
            var account = await sender.Send(new DepositToAccountCommand(id, body.Amount), cancellationToken);
            return account is null ? Results.NotFound() : Results.Ok(account);
        })
            .Produces<AccountDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("{id:guid}/withdraw", async (Guid id, AccountAmountBody body, ISender sender, CancellationToken cancellationToken) =>
        {
            var account = await sender.Send(new WithdrawFromAccountCommand(id, body.Amount), cancellationToken);
            return account is null ? Results.NotFound() : Results.Ok(account);
        })
            .Produces<AccountDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("{id:guid}/activate", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
        {
            var account = await sender.Send(new ActivateAccountCommand(id), cancellationToken);
            return account is null ? Results.NotFound() : Results.Ok(account);
        })
            .RequireAuthorization("AdminOnly")
            .Produces<AccountDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("{id:guid}/deactivate", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
        {
            var account = await sender.Send(new DeactivateAccountCommand(id), cancellationToken);
            return account is null ? Results.NotFound() : Results.Ok(account);
        })
            .RequireAuthorization("AdminOnly")
            .Produces<AccountDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);
    }
}

public record CreateAccountBody(Guid CustomerId, decimal Balance, string Currency);

public record UpdateAccountBody(string Currency);

public record AccountAmountBody(decimal Amount);
