using AutoMapper;
using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Dtos.Responses;
using StellarWallet.Application.Interfaces;
using StellarWallet.Domain.Entities;
using StellarWallet.Domain.Errors;
using StellarWallet.Domain.Interfaces.Persistence;
using StellarWallet.Domain.Interfaces.Services;
using StellarWallet.Domain.Result;

namespace StellarWallet.Application.Services
{
    public class UserContactService(IUserService userService, IJwtService jwtService, IMapper mapper, IAuthService authService, IUnitOfWork unitOfWork) : IUserContactService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IUserService _userService = userService;
        private readonly IJwtService _jwtService = jwtService;
        private readonly IAuthService _authService = authService;
        private readonly IMapper _mapper = mapper;

        public async Task<Result<bool, DomainError>> Add(AddContactDto userContact, string jwt)
        {
            var userEmailDecoding = _jwtService.DecodeToken(jwt);

            if (!userEmailDecoding.IsSuccess)
            {
                return Result<bool, DomainError>.Failure(userEmailDecoding.Error);
            }

            User? foundUser = await _unitOfWork.User.GetBy("Email", userEmailDecoding.Value);

            if (foundUser is null)
            {
                return Result<bool, DomainError>.Failure(DomainError.NotFound("User not found"));
            }

            var IsValidEmail = _authService.AuthenticateEmail(jwt, foundUser.Email);

            if (!IsValidEmail.IsSuccess)
            {
                return Result<bool, DomainError>.Failure(IsValidEmail.Error);
            }

            if (foundUser.UserContacts?.Count >= 10)
                throw new Exception("User has reached the maximum number of contacts");

            if (foundUser.UserContacts is not null)
                foreach (UserContact contact in foundUser.UserContacts)
                {
                    if (contact.Alias == userContact.Alias)
                        throw new Exception("Contact already exists");
                }

            await _unitOfWork.UserContact.Add(new UserContact(userContact.Alias, foundUser.Id, userContact.PublicKey));

            return Result<bool, DomainError>.Success(IsValidEmail.IsSuccess);
        }

        public async Task<Result<bool, DomainError>> Delete(int id)
        {
            UserContact? userContactFound = await _unitOfWork.UserContact.GetById(id);

            if (userContactFound is null)
            {
                return Result<bool, DomainError>.Failure(DomainError.NotFound("Contact not found"));
            }

            await _unitOfWork.UserContact.Delete(id);

            return Result<bool, DomainError>.Success(true);
        }

        public async Task<Result<IEnumerable<UserContactsDto>, DomainError>> GetAll(int userId, string jwt)
        {
            var foundUser = await _userService.GetById(userId, jwt);
            if (foundUser is null)
            {
                return Result<IEnumerable<UserContactsDto>, DomainError>.Failure(DomainError.NotFound("User not found"));
            }

            var IsValidEmail = _authService.AuthenticateEmail(jwt, foundUser.Value.Email);

            if (!IsValidEmail.IsSuccess)
            {
                return Result<IEnumerable<UserContactsDto>, DomainError>.Failure(IsValidEmail.Error);
            }

            IEnumerable<UserContact> userContacts = await _unitOfWork.UserContact.GetAll(uc => uc.UserId == userId);

            if (userContacts is null)
            {
                return Result<IEnumerable<UserContactsDto>, DomainError>.Failure(DomainError.NotFound("Contacts not found"));
            }

            var mappedUserContacts = _mapper.Map<UserContactsDto[]>(userContacts);

            return Result<IEnumerable<UserContactsDto>, DomainError>.Success(mappedUserContacts);
        }

        public async Task<Result<bool, DomainError>> Update(UpdateContactDto userContact)
        {
            UserContact? foundUserContact = await _unitOfWork.UserContact.GetById(userContact.Id);

            if (foundUserContact is null)
            {
                return Result<bool, DomainError>.Failure(DomainError.NotFound("Contact not found"));
            }

            if (userContact.Alias is not null)
            {
                foundUserContact.Alias = userContact.Alias;
            }

            _unitOfWork.UserContact.Update(foundUserContact);

            return Result<bool, DomainError>.Success(true);
        }
    }
}
