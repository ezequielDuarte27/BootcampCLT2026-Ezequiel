using CleanArchitecture.Full.Domain;
using CleanArchitecture.Full.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Full.Infrastructure.Repositories;

public class TransactionRepository(AppDbContext context) : ITransactionRepository
{
    public async Task AddAsync(Transaction transaction, CancellationToken cancellationToken = default) =>
        await context.Transactions.AddAsync(transaction, cancellationToken);

    public async Task<IReadOnlyList<Transaction>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default) =>
        await context.Transactions.AsNoTracking()
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
}
