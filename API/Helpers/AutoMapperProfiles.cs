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
            AllowNullDestinationValues = true;
            CreateMap<UserFromAuth0Dto, User>();
            CreateMap<User, UserForProfileViewDto>();
            CreateMap<ImageFromUserDto, DataForProfilePicUpdateDto>();
        }
    }
}