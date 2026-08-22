namespace CleanArchitecture.Full.Domain;

public class Account
{
    public Guid Id { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public decimal Balance { get; set; }
    public string Currency { get; set; } = "PYG";
    public string Status { get; set; } = "Active";
    public DateTime CreatedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
}
