using CleanArchitecture.Full.Application.Customers;
using CleanArchitecture.Full.Application.Customers.Commands.CreateCustomer;
using CleanArchitecture.Full.Application.Customers.Queries.GetCustomerById;
using MediatR;

namespace CleanArchitecture.Full.Api.Endpoints;

public static class CustomerEndpoints
{
    public static void MapCustomerEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/v1/customers").WithTags("Customers").RequireAuthorization();

        group.MapPost("", async (CreateCustomerCommand command, ISender sender, CancellationToken cancellationToken) =>
        {
            var customer = await sender.Send(command, cancellationToken);
            return Results.Created($"api/v1/customers/{customer.Id}", customer);
        })
            .RequireAuthorization("AdminOnly")
            .Produces<CustomerDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status409Conflict);

        group.MapGet("{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
        {
            var customer = await sender.Send(new GetCustomerByIdQuery(id), cancellationToken);
            return customer is null ? Results.NotFound() : Results.Ok(customer);
        })
            .Produces<CustomerDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);
    }
}
