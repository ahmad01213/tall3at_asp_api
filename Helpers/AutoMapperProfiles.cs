using AutoMapper;
using Donia.Dtos;
using Donia.Models;

namespace Donia.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<UserForRegister, User>();
            CreateMap<AddNewAdRequest, Ad>();
            CreateMap<TripAdd, Trib>();
            CreateMap<Trib, UserTripResponse>();
            CreateMap<AddService,Service >();
        }
    }
}
