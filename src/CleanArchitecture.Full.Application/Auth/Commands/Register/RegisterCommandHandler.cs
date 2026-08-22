using CleanArchitecture.Full.Domain;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Full.Application.Auth.Commands.Register;

public class RegisterCommandHandler(
    IUserRepository userRepository,
    ICustomerRepository customerRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator tokenGenerator,
    ILogger<RegisterCommandHandler> logger)
    : IRequestHandler<RegisterCommand, AuthResultDto>
{
    public async Task<AuthResultDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var customer = await customerRepository.GetByIdAsync(request.CustomerId, cancellationToken)
            ?? throw new ValidationException([
                new ValidationFailure(nameof(RegisterCommand.CustomerId), "El cliente indicado no existe.")
            ]);

        if (customer.DocumentNumber != request.DocumentNumber)
        {
            throw new ValidationException([
                new ValidationFailure(nameof(RegisterCommand.DocumentNumber), "El documento no coincide con el cliente indicado.")
            ]);
        }

        if (await userRepository.ExistsWithUsernameAsync(request.Username, cancellationToken))
        {
            throw new ValidationException([
                new ValidationFailure(nameof(RegisterCommand.Username), "El nombre de usuario ya está en uso.")
            ]);
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            PasswordHash = passwordHasher.Hash(request.Password),
            Role = UserRoles.Cliente,
            CustomerId = customer.Id,
            CreatedAt = DateTime.UtcNow
        };

        await userRepository.AddAsync(user, cancellationToken);
        await userRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Usuario {Username} registrado para el cliente {CustomerId}", user.Username, customer.Id);

        var token = tokenGenerator.GenerateToken(user.Id, user.Username, user.Role, user.CustomerId);

        return new AuthResultDto(token.Token, user.Username, user.Role, user.CustomerId, token.ExpiresAtUtc);
    }
}
