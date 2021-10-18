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
            CreateMap<MarketForAddDto, Market>();
            CreateMap<FoodForAdd, Food>();

            CreateMap<User, UserDetailResponse>();
            CreateMap<DriverForRegister, User>();
            CreateMap<DriverForRegister, Driver>();

        }
    }
}
