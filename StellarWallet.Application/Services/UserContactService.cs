using AutoMapper;
using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Dtos.Responses;
using StellarWallet.Application.Interfaces;
using StellarWallet.Domain.Entities;
using StellarWallet.Domain.Interfaces.Persistence;
using StellarWallet.Domain.Interfaces.Services;

namespace StellarWallet.Application.Services
{
    public class UserContactService(IUserService userService, IJwtService jwtService, IMapper mapper, IAuthService authService, IUnitOfWork unitOfWork) : IUserContactService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IUserService _userService = userService;
        private readonly IJwtService _jwtService = jwtService;
        private readonly IAuthService _authService = authService;
        private readonly IMapper _mapper = mapper;

        private void AuthenticateUserEmail(string jwt, string email)
        {
            bool isAnAuthorizedUser = _authService.AuthenticateEmail(jwt, email);
            if (!isAnAuthorizedUser) throw new Exception("Unauthorized");
        }

        public async Task Add(AddContactDto userContact, string jwt)
        {
            string userEmail = _jwtService.DecodeToken(jwt);

            User foundUser = await _unitOfWork.User.GetBy("Email", userEmail) ?? throw new Exception("User not found");

            AuthenticateUserEmail(jwt, foundUser.Email);

            if (foundUser.UserContacts?.Count >= 10)
                throw new Exception("User has reached the maximum number of contacts");

            if (foundUser.UserContacts is not null)
                foreach (UserContact contact in foundUser.UserContacts)
                {
                    if (contact.Alias == userContact.Alias)
                        throw new Exception("Contact already exists");
                }

            await _unitOfWork.UserContact.Add(new UserContact(userContact.Alias, foundUser.Id, userContact.PublicKey));
        }

        public async Task Delete(int id)
        {
            await _unitOfWork.UserContact.Delete(id);
        }

        public async Task<IEnumerable<UserContactsDto>> GetAll(int userId, string jwt)
        {
            var foundUser = await _userService.GetById(userId, jwt) ?? throw new Exception("User not found");

            AuthenticateUserEmail(jwt, foundUser.Email);

            IEnumerable<UserContact> userContacts = await _unitOfWork.UserContact.GetAll(uc => uc.UserId == userId);

            return _mapper.Map<UserContactsDto[]>(userContacts);
        }

        public async Task Update(UpdateContactDto userContact)
        {
            UserContact foundUserContact = await _unitOfWork.UserContact.GetById(userContact.Id) ?? throw new Exception("Contact not found");
            if (userContact.Alias is not null)
                foundUserContact.Alias = userContact.Alias;

            _unitOfWork.UserContact.Update(foundUserContact);
        }
    }
}
