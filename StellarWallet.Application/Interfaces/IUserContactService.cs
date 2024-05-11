using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Dtos.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellarWallet.Application.Interfaces
{
    public  interface IUserContactService
    {
        Task<IEnumerable<UserContactsDto>> GetAll(int id);
        Task Add(AddContactDto userContact);
        Task Update(UpdateContactDto userContact);
        Task Delete(int id);
    }
}
