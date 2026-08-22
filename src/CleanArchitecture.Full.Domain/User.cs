namespace CleanArchitecture.Full.Domain;

public static class UserRoles
{
    public const string Admin = "Admin";
    public const string Cliente = "Cliente";
}

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = UserRoles.Cliente;
    public Guid? CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public DateTime CreatedAt { get; set; }
}
