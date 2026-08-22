using MediatR;

namespace CleanArchitecture.Full.Application.Auth.Commands.Register;

public record RegisterCommand(string Username, string Password, Guid CustomerId, string DocumentNumber) : IRequest<AuthResultDto>;
