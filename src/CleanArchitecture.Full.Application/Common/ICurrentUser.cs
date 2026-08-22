namespace CleanArchitecture.Full.Application.Common;

public interface ICurrentUser
{
    bool IsAuthenticated { get; }
    bool IsAdmin { get; }
    Guid? CustomerId { get; }
}
