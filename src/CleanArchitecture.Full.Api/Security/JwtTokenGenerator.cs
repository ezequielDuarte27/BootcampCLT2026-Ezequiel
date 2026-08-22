using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CleanArchitecture.Full.Domain;
using Microsoft.IdentityModel.Tokens;

namespace CleanArchitecture.Full.Api.Security;

public class JwtTokenGenerator(IConfiguration configuration) : IJwtTokenGenerator
{
    public GeneratedToken GenerateToken(Guid userId, string username, string role, Guid? customerId)
    {
        var signingKey = configuration["Jwt:SigningKey"]
            ?? throw new InvalidOperationException("Jwt:SigningKey no está configurado.");
        var expirationMinutes = configuration.GetValue<int?>("Jwt:ExpirationMinutes") ?? 60;
        var expiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(ClaimTypes.Name, username),
            new(ClaimTypes.Role, role)
        };

        if (customerId.HasValue)
        {
            claims.Add(new Claim("customerId", customerId.Value.ToString()));
        }

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new GeneratedToken(new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
