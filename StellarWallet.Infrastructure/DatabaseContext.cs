using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StellarWallet.Domain.Entities;
using StellarWallet.Infrastructure.Utilities;

namespace StellarWallet.Infrastructure.DatabaseConnection
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<BlockchainAccount> BlockchainAccounts { get; set; }
        public DbSet<UserContact> UserContacts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<BlockchainAccount>()
                .HasIndex(b => b.PublicKey)
                .IsUnique();

            modelBuilder.Entity<UserContact>()
                .HasIndex(uc => uc.Id)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasMany(u => u.BlockchainAccounts)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.UserContacts)
                .WithOne(uc => uc.User)
                .HasForeignKey(uc => uc.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "test";
            string connectionString = AppSettingsVariables.GetConnectionString(environment);

            if (!string.IsNullOrEmpty(connectionString))
                optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
