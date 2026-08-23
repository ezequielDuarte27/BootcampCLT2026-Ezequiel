using CleanArchitecture.Full.Domain;

namespace CleanArchitecture.Full.Application.Tests.TestDoubles;

internal sealed class InMemoryAccountRepository : IAccountRepository
{
    private readonly Dictionary<Guid, Account> _accounts = [];

    public void Seed(Account account) => _accounts[account.Id] = account;

    public Task<IReadOnlyList<Account>> GetAllAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Account>>(_accounts.Values.ToList());

    public Task<IReadOnlyList<Account>> GetAllByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Account>>(_accounts.Values.Where(a => a.CustomerId == customerId).ToList());

    public Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(_accounts.GetValueOrDefault(id));

    public Task<Account?> GetByAccountNumberAsync(string accountNumber, CancellationToken cancellationToken = default) =>
        Task.FromResult(_accounts.Values.FirstOrDefault(a => a.AccountNumber == accountNumber));

    public Task<bool> AccountNumberExistsAsync(string accountNumber, CancellationToken cancellationToken = default) =>
        Task.FromResult(_accounts.Values.Any(a => a.AccountNumber == accountNumber));

    public Task AddAsync(Account account, CancellationToken cancellationToken = default)
    {
        _accounts[account.Id] = account;
        return Task.CompletedTask;
    }

    public void Update(Account account) => _accounts[account.Id] = account;

    public Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromResult(true);
}
