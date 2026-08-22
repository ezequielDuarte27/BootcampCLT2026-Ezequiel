namespace CleanArchitecture.Full.Domain;

public class Customer
{
    public Guid Id { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string DocumentNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
