using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellarWallet.Application.Interfaces
{
    public interface IEncryptionService
    {
        string Encrypt(string text);
        bool Verify(string text, string hash);
    }
}
