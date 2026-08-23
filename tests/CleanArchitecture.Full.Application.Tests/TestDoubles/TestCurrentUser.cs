using CleanArchitecture.Full.Application.Common;

namespace CleanArchitecture.Full.Application.Tests.TestDoubles;

internal sealed class TestCurrentUser : ICurrentUser
{
    public bool IsAuthenticated => true;

    public bool IsAdmin { get; init; } = true;

    public Guid? CustomerId { get; init; }
}
