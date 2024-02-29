using AutoMapper;
using StellarWallet.Application.Dtos.Responses;
using StellarWallet.Domain.Entities;

namespace StellarWallet.Application.Mappers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>();
        }
    }
}
