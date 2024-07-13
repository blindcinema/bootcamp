using AutoMapper;
using TrackIt.Models;

namespace TrackIt.WebAPI.AutoMapperProfiles
{
    public class DeliveryStatusProfile : Profile
    {
        public DeliveryStatusProfile()
        {
            CreateMap<DeliveryStatusDto, DeliveryStatus>();

        }
    }
}
