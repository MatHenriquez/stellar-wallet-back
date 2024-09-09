using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Dtos.Responses;
using StellarWallet.Domain.Errors;
using StellarWallet.Domain.Result;

namespace StellarWallet.Application.Interfaces
{
    public interface IUserContactService
    {
        Task<Result<IEnumerable<UserContactsDto>, DomainError>> GetAll(int id, string jwt);
        Task<Result<bool, DomainError>> Add(AddContactDto userContact, string jwt);
        Task<Result<bool, DomainError>> Update(UpdateContactDto userContact);
        Task<Result<bool, DomainError>> Delete(int id);
    }
}
