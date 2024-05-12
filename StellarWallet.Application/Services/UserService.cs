using AutoMapper;
using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Dtos.Responses;
using StellarWallet.Application.Interfaces;
using StellarWallet.Domain.Entities;
using StellarWallet.Domain.Interfaces;
using StellarWallet.Domain.Repositories;
using StellarWallet.Domain.Structs;

namespace StellarWallet.Application.Services
{
    public class UserService(IUserRepository userRepository, IJwtService jwtService, IEncryptionService encryptionService, IMapper mapper, IBlockchainService stellarService, IBlockchainAccountRepository blockchainAccountRepository
        ) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IJwtService _jwtService = jwtService;
        private readonly IEncryptionService _encryptionService = encryptionService;
        private readonly IBlockchainService _stellarService = stellarService;
        private readonly IBlockchainAccountRepository _blockchainAccountRepository = blockchainAccountRepository;
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

        public async Task<LoggedDto> Add(UserCreationDto user)
        {
            User? foundUser = await _userRepository.GetBy("Email", user.Email);
            if (foundUser is not null)
                throw new Exception("User already exists");

            user.Password = _encryptionService.Encrypt(user.Password);

            AccountKeyPair account = _stellarService.CreateKeyPair();
            user.PublicKey = account.PublicKey;
            user.SecretKey = account.SecretKey;

            await _userRepository.Add(_mapper.Map<User>(user));

            string createdToken = _jwtService.CreateToken(user.Email);

            return new LoggedDto(true, createdToken, user.PublicKey);
        }

        public async Task Update(UserUpdateDto user)
        {
            if (user.Password is not null)
                user.Password = _encryptionService.Encrypt(user.Password);

            await _userRepository.GetById(user.Id);
            await _userRepository.Update(_mapper.Map<User>(user));
        }

        public async Task Delete(int id)
        {
            await _userRepository.GetById(id);
            await _userRepository.Delete(id);
        }

        public async Task AddWallet(AddWalletDto wallet, string jwt)
        {
            try
            {
                string email = _jwtService.DecodeToken(jwt);
                User foundUser = await _userRepository.GetBy("Email", email) ?? throw new Exception("User not found");

                foreach (BlockchainAccount account in foundUser.BlockchainAccounts)
                {
                    if (account.PublicKey == wallet.PublicKey)
                        throw new Exception("Wallet already exists");
                }

                BlockchainAccount newAccount = new(wallet.PublicKey, wallet.SecretKey, foundUser.Id)
                {
                    User = foundUser
                };
                await _blockchainAccountRepository.Add(newAccount);
                await _userRepository.Update(foundUser);
            }
            catch (Exception e)
            {
                throw new Exception("Error adding wallet: " + e.Message);
            }
        }
    }
}
