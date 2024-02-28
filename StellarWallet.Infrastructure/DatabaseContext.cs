using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StellarWallet.Domain.Entities;

namespace StellarWallet.Infrastructure.DatabaseConnection
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configuration = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", optional: false)
              .Build();

            var path = Directory.GetCurrentDirectory();

            var connectionString = configuration.GetConnectionString("StellarWallet");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
