using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Dtos.Responses;

namespace StellarWallet.Application.Interfaces
{
    public interface IUserContactService
    {
        Task<IEnumerable<UserContactsDto>> GetAll(int id, string jwt);
        Task Add(AddContactDto userContact, string jwt);
        Task Update(UpdateContactDto userContact);
        Task Delete(int id);
    }
}
