using CleanArchitecture.Full.Domain;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Full.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("customers");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.DocumentType).IsRequired().HasMaxLength(20);
            entity.Property(c => c.DocumentNumber).IsRequired().HasMaxLength(20);
            entity.Property(c => c.FullName).IsRequired().HasMaxLength(150);
            entity.HasIndex(c => new { c.DocumentType, c.DocumentNumber }).IsUnique();
        });

        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("accounts");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.AccountNumber).IsRequired().HasMaxLength(20);
            entity.Property(a => a.Balance).HasColumnType("numeric(18,2)");
            entity.Property(a => a.Currency).IsRequired().HasMaxLength(3);
            entity.Property(a => a.Status).IsRequired().HasMaxLength(20);
            entity.HasIndex(a => a.AccountNumber).IsUnique();
            entity.HasOne(a => a.Customer)
                .WithMany()
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.ToTable("transactions");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Type).IsRequired().HasMaxLength(20);
            entity.Property(t => t.Amount).HasColumnType("numeric(18,2)");
            entity.Property(t => t.Currency).IsRequired().HasMaxLength(3);
            entity.Property(t => t.BalanceAfter).HasColumnType("numeric(18,2)");
            entity.Property(t => t.Description).HasMaxLength(250);
            entity.HasOne(t => t.Account)
                .WithMany()
                .HasForeignKey(t => t.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(t => t.AccountId);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Username).IsRequired().HasMaxLength(50);
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.Role).IsRequired().HasMaxLength(20);
            entity.HasIndex(u => u.Username).IsUnique();
            entity.HasOne(u => u.Customer)
                .WithMany()
                .HasForeignKey(u => u.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
