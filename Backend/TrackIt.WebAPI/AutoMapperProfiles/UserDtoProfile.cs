using AutoMapper;
using TrackIt.Models;

namespace TrackIt.WebAPI.AutoMapperProfiles
{
    public class UserDtoProfile : Profile
    {
        public UserDtoProfile()
        {
            CreateMap<CreateUserDto, UserDto>();
            CreateMap<LoginUserDto, UserDto>();
            CreateMap<UserDto, User>();
            CreateMap<EditUserDto, UserDto>();
        }
    }
}
