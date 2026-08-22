using CleanArchitecture.Full.Application.Common;

namespace CleanArchitecture.Full.Api.Security;

public class AdminCredentials(IConfiguration configuration) : IAdminCredentials
{
    public string Username => configuration["Auth:AdminUsername"] ?? "admin";

    public string Password => configuration["Auth:AdminPassword"]
        ?? throw new InvalidOperationException("Auth:AdminPassword no está configurado.");
}
