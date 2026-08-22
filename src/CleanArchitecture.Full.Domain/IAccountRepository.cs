namespace CleanArchitecture.Full.Domain;

public interface IAccountRepository
{
    Task<IReadOnlyList<Account>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Account>> GetAllByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Account?> GetByAccountNumberAsync(string accountNumber, CancellationToken cancellationToken = default);
    Task<bool> AccountNumberExistsAsync(string accountNumber, CancellationToken cancellationToken = default);
    Task AddAsync(Account account, CancellationToken cancellationToken = default);
    void Update(Account account);
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
}
