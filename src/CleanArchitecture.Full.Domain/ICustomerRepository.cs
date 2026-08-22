namespace CleanArchitecture.Full.Domain;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Customer customer, CancellationToken cancellationToken = default);
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
}
