using MediatR;

namespace CleanArchitecture.Full.Application.Auth.Commands.Login;

public record LoginCommand(string Username, string Password) : IRequest<AuthResultDto?>;
