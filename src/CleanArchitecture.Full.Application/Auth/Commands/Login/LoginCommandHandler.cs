using CleanArchitecture.Full.Application.Common;
using CleanArchitecture.Full.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Full.Application.Auth.Commands.Login;

public class LoginCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator tokenGenerator,
    IAdminCredentials adminCredentials,
    ILogger<LoginCommandHandler> logger)
    : IRequestHandler<LoginCommand, AuthResultDto?>
{
    public async Task<AuthResultDto?> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        if (request.Username == adminCredentials.Username && request.Password == adminCredentials.Password)
        {
            var adminToken = tokenGenerator.GenerateToken(Guid.Empty, adminCredentials.Username, UserRoles.Admin, null);
            logger.LogInformation("Login de administrador exitoso para {Username}", adminCredentials.Username);
            return new AuthResultDto(adminToken.Token, adminCredentials.Username, UserRoles.Admin, null, adminToken.ExpiresAtUtc);
        }

        var user = await userRepository.GetByUsernameAsync(request.Username, cancellationToken);
        if (user is null || !passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            logger.LogWarning("Intento de login fallido para el usuario {Username}", request.Username);
            return null;
        }

        var token = tokenGenerator.GenerateToken(user.Id, user.Username, user.Role, user.CustomerId);

        logger.LogInformation("Login exitoso para {Username}", user.Username);

        return new AuthResultDto(token.Token, user.Username, user.Role, user.CustomerId, token.ExpiresAtUtc);
    }
}
