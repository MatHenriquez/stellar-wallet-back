﻿using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Dtos.Responses;
using StellarWallet.Application.Interfaces;
using StellarWallet.Domain.Entities;
using StellarWallet.Domain.Interfaces;
using StellarWallet.Domain.Repositories;

namespace StellarWallet.Application.Services
{
    public class UserContactService(IUserContactRepository userContactRepository, IUserService userService, IUserRepository userRepository, IJwtService jwtService) : IUserContactService
    {
        private readonly IUserContactRepository _userContactRepository = userContactRepository;
        private readonly IUserService _userService = userService;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IJwtService _jwtService = jwtService;

        public async Task Add(AddContactDto userContact, string jwt)
        {
            string userEmail = _jwtService.DecodeToken(jwt);
           User foundUser = await _userRepository.GetBy("Email", userEmail) ?? throw new Exception("User not found");
            await _userContactRepository.Add(new UserContact(userContact.Alias, foundUser.Id));
        }

        public async Task Delete(int id)
        {
            await _userContactRepository.Delete(id);
        }

        public async Task<IEnumerable<UserContactsDto>> GetAll(int userId)
        {
            var user = await _userService.GetById(userId) ?? throw new Exception("User not found");
            IEnumerable<UserContact> userContacts = await _userContactRepository.GetAll(userId);
            return userContacts.Select(uc => new UserContactsDto(uc.Alias, user.PublicKey));
        }

        public async Task Update(UpdateContactDto userContact)
        {
            UserContact foundUserContact = await _userContactRepository.GetById(userContact.Id);
            if (userContact.Alias is not null)
                foundUserContact.Alias = userContact.Alias;

            await _userContactRepository.Update(foundUserContact);
        }
    }
}
