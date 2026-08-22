using CleanArchitecture.Full.Domain;

namespace CleanArchitecture.Full.Infrastructure.Persistence;

public static class DbSeeder
{
    public static void Seed(AppDbContext context)
    {
        if (context.Accounts.Any())
        {
            return;
        }

        var lovelace = new Customer
        {
            Id = Guid.NewGuid(),
            DocumentType = "DNI",
            DocumentNumber = "10000001",
            FullName = "Ada Lovelace",
            CreatedAt = DateTime.UtcNow
        };

        var turing = new Customer
        {
            Id = Guid.NewGuid(),
            DocumentType = "DNI",
            DocumentNumber = "10000002",
            FullName = "Alan Turing",
            CreatedAt = DateTime.UtcNow
        };

        var hopper = new Customer
        {
            Id = Guid.NewGuid(),
            DocumentType = "DNI",
            DocumentNumber = "10000003",
            FullName = "Grace Hopper",
            CreatedAt = DateTime.UtcNow
        };

        context.Customers.AddRange(lovelace, turing, hopper);

        context.Accounts.AddRange(
            new Account
            {
                Id = Guid.NewGuid(),
                AccountNumber = "ACC-000001",
                CustomerId = lovelace.Id,
                Balance = 15000.50m,
                Currency = "ARS",
                Status = "Active",
                CreatedAt = DateTime.UtcNow
            },
            new Account
            {
                Id = Guid.NewGuid(),
                AccountNumber = "ACC-000002",
                CustomerId = turing.Id,
                Balance = 8320.00m,
                Currency = "ARS",
                Status = "Active",
                CreatedAt = DateTime.UtcNow
            },
            new Account
            {
                Id = Guid.NewGuid(),
                AccountNumber = "ACC-000003",
                CustomerId = hopper.Id,
                Balance = 0.00m,
                Currency = "ARS",
                Status = "Inactive",
                CreatedAt = DateTime.UtcNow
            });

        context.SaveChanges();
    }
}
