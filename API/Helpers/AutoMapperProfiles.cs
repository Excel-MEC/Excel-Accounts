using AutoMapper;
using API.Dtos.Auth;
using API.Models;

namespace API.Helpers
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