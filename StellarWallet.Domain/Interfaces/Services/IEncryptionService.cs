using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellarWallet.Domain.Interfaces.Services
{
    public interface IEncryptionService
    {
        string Encrypt(string text);
        bool Verify(string text, string hash);
    }
}
