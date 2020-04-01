using AutoMapper;
using API.Dtos.Auth;
using API.Models;
using API.Dtos.Profile;

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

            CreateMap<User, DataForProfileViewDto>()
                .ForMember(dest => dest.Id, opt =>
                {
                    opt.MapFrom(src => src.Id);
                })
                .ForMember(dest => dest.Name, opt =>
                {
                    opt.MapFrom(src => src.Name);
                })
                .ForMember(dest => dest.Email, opt =>
                {
                    opt.MapFrom(src => src.Email);
                })                
                .ForMember(dest => dest.Picture, opt =>
                {
                    opt.MapFrom(src => src.Picture);
                })
                .ForMember(dest => dest.QRCodeUrl, opt =>
                {
                    opt.MapFrom(src => src.QRCodeUrl);
                })
                .ForMember(dest => dest.Gender, opt =>
                {
                    opt.MapFrom(src => src.Gender);
                })
                .ForMember(dest => dest.MobileNumber, opt =>
                {
                    opt.MapFrom(src => src.MobileNumber);
                })
                .ForMember(dest => dest.Category, opt =>
                {
                    opt.MapFrom(src => src.Category);
                });
        }
    }
}