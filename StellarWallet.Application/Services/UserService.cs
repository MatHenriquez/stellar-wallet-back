using AutoMapper;
using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Dtos.Responses;
using StellarWallet.Application.Interfaces;
using StellarWallet.Domain.Entities;
using StellarWallet.Domain.Repositories;

namespace StellarWallet.Application.Services
{
    public class UserService(IUserRepository userRepository, IMapper mapper) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<UserDto>> GetAll()
        {
            IEnumerable<User> users = await _userRepository.GetAll();
            return _mapper.Map<UserDto[]>(users);
        }

        public async Task<UserDto> GetById(int id)
        {
            User foundUser = await _userRepository.GetById(id);
            return _mapper.Map<UserDto>(foundUser);
        }

        public async Task Add(UserCreationDto user)
        {
            User? foundUser = await _userRepository.GetBy("Email", user.Email);
            if (foundUser != null)
                throw new Exception("User already exists");

            await _userRepository.Add(_mapper.Map<User>(user));
        }

        public async Task Update(UserUpdateDto user)
        {
            await _userRepository.GetById(user.Id);
            await _userRepository.Update(_mapper.Map<User>(user));
        }

        public async Task Delete(int id)
        {
            await _userRepository.GetById(id);
            await _userRepository.Delete(id);
        }
    }
}
