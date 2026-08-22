using CleanArchitecture.Full.Application.Auth;
using CleanArchitecture.Full.Application.Auth.Commands.Login;
using CleanArchitecture.Full.Application.Auth.Commands.Register;
using MediatR;

namespace CleanArchitecture.Full.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/v1/auth").WithTags("Auth");

        group.MapPost("login", async (LoginCommand command, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);
            return result is null ? Results.Unauthorized() : Results.Ok(result);
        })
            .AllowAnonymous()
            .Produces<AuthResultDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("register", async (RegisterCommand command, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);
            return Results.Created("api/v1/auth/login", result);
        })
            .AllowAnonymous()
            .Produces<AuthResultDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);
    }
}
