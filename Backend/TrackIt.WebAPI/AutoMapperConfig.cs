using AutoMapper;
using TrackIt.Models;
using TrackIt.WebAPI.AutoMapperProfiles;
using TrackIt.WebAPI.Model;

namespace TrackIt.WebAPI
{
    public class AutoMapperConfig
    {

        MapperConfiguration config = new MapperConfiguration(cfg => {
            cfg.CreateMap<UserDto, User>();
            cfg.AddProfile<UserDtoProfile>();
            cfg.CreateMap<ClientDto, Client>();
            cfg.CreateMap<CourierDto, Courier>();
        });
    }
}
