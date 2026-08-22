namespace CleanArchitecture.Full.Application.Auth;

public record AuthResultDto(string Token, string Username, string Role, Guid? CustomerId, DateTime ExpiresAtUtc);
