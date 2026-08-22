using CleanArchitecture.Full.Application.Accounts.Commands.TransferBetweenAccounts;
using MediatR;

namespace CleanArchitecture.Full.Api.Endpoints;

public static class TransferEndpoints
{
    public static void MapTransferEndpoints(this WebApplication app)
    {
        app.MapPost("api/v1/transfers", async (TransferRequestBody body, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new TransferBetweenAccountsCommand(
                new TransferSender(body.Sender.AccountNumber, body.Sender.DocumentNumber),
                new TransferBeneficiary(body.Beneficiary.AccountNumber, body.Beneficiary.DocumentType, body.Beneficiary.DocumentNumber),
                body.Amount,
                body.Currency);

            var result = await sender.Send(command, cancellationToken);
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
            .WithTags("Transfers")
            .RequireAuthorization()
            .Produces<TransferResultDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);
    }
}

public record TransferSenderBody(string AccountNumber, string DocumentNumber);

public record TransferBeneficiaryBody(string AccountNumber, string DocumentType, string DocumentNumber);

public record TransferRequestBody(TransferSenderBody Sender, TransferBeneficiaryBody Beneficiary, decimal Amount, string Currency);
