using Microsoft.EntityFrameworkCore;
using StellarWallet.Domain.Entities;
using StellarWallet.Domain.Interfaces;
using StellarWallet.Infrastructure.DatabaseConnection;

namespace StellarWallet.Infrastructure.Repositories
{
    public class BlockchainAccountRepository(DatabaseContext context) : IBlockchainAccountRepository
    {
        private readonly DatabaseContext _context = context;

        public async Task Add(BlockchainAccount blockchainAccount)
        {
            try { 
            await _context.BlockchainAccounts.AddAsync(blockchainAccount);
            await _context.SaveChangesAsync();
                } catch (Exception e)
            {
                throw new Exception("Error adding blockchain account", e);
            }
        }
    }
}
