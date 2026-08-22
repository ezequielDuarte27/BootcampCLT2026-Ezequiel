using CleanArchitecture.Full.Domain;
using CleanArchitecture.Full.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Full.Infrastructure.Repositories;

public class CustomerRepository(AppDbContext context) : ICustomerRepository
{
    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await context.Customers.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task AddAsync(Customer customer, CancellationToken cancellationToken = default) =>
        await context.Customers.AddAsync(customer, cancellationToken);

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await context.SaveChangesAsync(cancellationToken) >= 0;
}
