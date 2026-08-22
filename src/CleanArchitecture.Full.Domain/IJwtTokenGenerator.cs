namespace CleanArchitecture.Full.Domain;

public record GeneratedToken(string Token, DateTime ExpiresAtUtc);

public interface IJwtTokenGenerator
{
    GeneratedToken GenerateToken(Guid userId, string username, string role, Guid? customerId);
}
