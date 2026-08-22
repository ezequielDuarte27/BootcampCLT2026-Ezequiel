using CleanArchitecture.Full.Domain;
using CleanArchitecture.Full.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Full.Infrastructure.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default) =>
        await context.Users.Include(u => u.Customer).FirstOrDefaultAsync(u => u.Username == username, cancellationToken);

    public async Task<bool> ExistsWithUsernameAsync(string username, CancellationToken cancellationToken = default) =>
        await context.Users.AnyAsync(u => u.Username == username, cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken = default) =>
        await context.Users.AddAsync(user, cancellationToken);

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await context.SaveChangesAsync(cancellationToken) >= 0;
}
