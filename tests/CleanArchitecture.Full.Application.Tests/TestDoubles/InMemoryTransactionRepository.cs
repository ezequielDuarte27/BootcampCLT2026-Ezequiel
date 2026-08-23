using CleanArchitecture.Full.Domain;

namespace CleanArchitecture.Full.Application.Tests.TestDoubles;

internal sealed class InMemoryTransactionRepository : ITransactionRepository
{
    public List<Transaction> Transactions { get; } = [];

    public Task AddAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        Transactions.Add(transaction);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Transaction>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Transaction>>(Transactions.Where(t => t.AccountId == accountId).ToList());
}
