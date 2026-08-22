using CleanArchitecture.Full.Domain;
using CleanArchitecture.Full.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Full.Infrastructure.Repositories;

public class AccountRepository(AppDbContext context) : IAccountRepository
{
    public async Task<IReadOnlyList<Account>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await context.Accounts.Include(a => a.Customer).AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Account>> GetAllByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default) =>
        await context.Accounts.Include(a => a.Customer).AsNoTracking()
            .Where(a => a.CustomerId == customerId)
            .ToListAsync(cancellationToken);

    public async Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await context.Accounts.Include(a => a.Customer).FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<Account?> GetByAccountNumberAsync(string accountNumber, CancellationToken cancellationToken = default) =>
        await context.Accounts.Include(a => a.Customer).FirstOrDefaultAsync(a => a.AccountNumber == accountNumber, cancellationToken);

    public async Task<bool> AccountNumberExistsAsync(string accountNumber, CancellationToken cancellationToken = default) =>
        await context.Accounts.AnyAsync(a => a.AccountNumber == accountNumber, cancellationToken);

    public async Task AddAsync(Account account, CancellationToken cancellationToken = default) =>
        await context.Accounts.AddAsync(account, cancellationToken);

    public void Update(Account account) => context.Accounts.Update(account);

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await context.SaveChangesAsync(cancellationToken) >= 0;
}
