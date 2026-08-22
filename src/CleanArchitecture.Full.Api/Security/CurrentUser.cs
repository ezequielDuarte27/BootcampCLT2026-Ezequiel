using System.Security.Claims;
using CleanArchitecture.Full.Application.Common;

namespace CleanArchitecture.Full.Api.Security;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private ClaimsPrincipal? Principal => httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated == true;

    public bool IsAdmin => Principal?.IsInRole("Admin") == true;

    public Guid? CustomerId
    {
        get
        {
            var value = Principal?.FindFirst("customerId")?.Value;
            return Guid.TryParse(value, out var customerId) ? customerId : null;
        }
    }
}
