using AutoMapper;
using API.Dtos.Auth;
using API.Models;
using API.Dtos.Profile;
using API.Dtos.Ambassador;
using Google.Apis.Auth;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            AllowNullDestinationValues = true;
            CreateMap<UserFromAuth0Dto, User>();
            CreateMap<User, UserForProfileViewDto>();
            CreateMap<ImageFromUserDto, DataForProfilePicUpdateDto>();
            CreateMap<User, AmbassadorListViewDto>();
            CreateMap<User, AmbassadorProfileDto>();
            CreateMap<User, UserViewDto>();
            CreateMap<GoogleJsonWebSignature.Payload, User>();         
        }
    }
}