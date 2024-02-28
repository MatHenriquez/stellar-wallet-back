using Microsoft.EntityFrameworkCore;
using StellarWallet.Domain.Entities;

namespace StellarWallet.Infrastructure
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\CFServer;Database=StellarWallet;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True");
        }

    }
}
