using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StellarWallet.Domain.Entities;

namespace StellarWallet.Infrastructure.DatabaseConnection
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<BlockchainAccount> BlockchainAccounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>().HasMany(u => u.BlockchainAccounts)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId);

            modelBuilder.Entity<BlockchainAccount>()
                .HasIndex(b => b.PublicKey)
                .IsUnique();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configuration = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", optional: true)
              .Build();

            var path = Directory.GetCurrentDirectory();
            var connectionString = configuration.GetConnectionString("StellarWallet");

            if (!string.IsNullOrEmpty(connectionString))
            {
                optionsBuilder.UseSqlServer(connectionString);
            }
            else
            {
                configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.test.json", optional: true)
               .Build();

                path = Directory.GetCurrentDirectory();
                connectionString = configuration.GetConnectionString("StellarWallet");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
    }
}
