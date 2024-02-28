using StellarWallet.Application.Interfaces;
using StellarWallet.Domain.Entities;
using StellarWallet.Domain.Repositories;

namespace StellarWallet.Application.Services
{
    public class UserService(IUserRepository userRepository) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<IEnumerable<User>> GetAll()
        {
            return await _userRepository.GetAll();
        }

        public async Task<User> GetById(int id)
        {
            return await _userRepository.GetById(id);
        }

        public async Task Add(User user)
        {
            await _userRepository.Add(user);
        }

        public async Task Update(User user)
        {
            await _userRepository.Update(user);
        }

        public async Task Delete(int id)
        {
            await _userRepository.Delete(id);
        }
    }
}
