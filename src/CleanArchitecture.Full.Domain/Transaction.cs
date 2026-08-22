namespace CleanArchitecture.Full.Domain;

public static class TransactionTypes
{
    public const string Deposit = "Deposit";
    public const string Withdrawal = "Withdrawal";
    public const string TransferOut = "TransferOut";
    public const string TransferIn = "TransferIn";
}

public class Transaction
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public Account? Account { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public decimal BalanceAfter { get; set; }
    public Guid? RelatedAccountId { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
