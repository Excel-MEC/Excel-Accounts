using AutoMapper;
using Excel_Accounts_Backend.Dtos.Auth;
using Excel_Accounts_Backend.Models;

namespace Excel_Accounts_Backend.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<UserFromAuth0Dto, User>()
                .ForMember(dest => dest.Name, opt =>
                {
                    opt.MapFrom(src => src.name);
                })
                .ForMember(dest => dest.Email, opt =>
                {
                    opt.MapFrom(src => src.email);
                });
        }
    }
}