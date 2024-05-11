using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StellarWallet.Domain.Entities;

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
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "test";

            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory());

            if (File.Exists($"appsettings.{environmentName}.json"))
            {
                configurationBuilder.AddJsonFile($"appsettings.{environmentName}.json", optional: false);
            }
            else
            {
                configurationBuilder.AddJsonFile("appsettings.json", optional: false);

            }

            var configuration = configurationBuilder.Build();

            var connectionString = configuration.GetConnectionString("StellarWallet");

            if (!string.IsNullOrEmpty(connectionString))
            {
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

    }
}
