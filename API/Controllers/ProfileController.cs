using System.Linq;
using System.Threading.Tasks;
using API.Dtos.Profile;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Data.Interfaces;
using API.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers
{
    [SwaggerTag("All the routes under this controller needs Authorization header.")]
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileRepository _repo;
        private readonly IMapper _mapper;
        private readonly IInstitutionRepository _institution;
        private readonly IProfileService _profileService;
        public ProfileController(IProfileRepository repo, IMapper mapper, IInstitutionRepository institution, IProfileService profileService)
        {
            _institution = institution;
            _mapper = mapper;
            _repo = repo;
            _profileService = profileService;
        }
        [SwaggerOperation(Description = "Raw User data stored in the database. Useful for editing profile")]
        [HttpGet]
        public async Task<ActionResult<User>> Get()
        {
            var id = int.Parse(User.Claims.First(i => i.Type == "user_id").Value);
            var user = await _repo.GetUser(id);
            return Ok(user);
        }

        [SwaggerOperation(
            Description = "This route is for Updating User Profile. For category, use (college/school/professional). For the institution id, obtain the list of institution from the backend and send the id of the institution the user select.\n If The user wants to add a new institution, Set the instution id to 0  and set the institution name as the Name of new institution. Institution name and id are not applicable for professional category"
        )]
        [HttpPost("update")]
        public async Task<ActionResult<User>> UpdateProfile(UserForProfileUpdateDto data)
        {
            var id = int.Parse(User.Claims.First(i => i.Type == "user_id").Value);
            var user = await _repo.UpdateProfile(id, data);
            return Ok(user);
        }

        [SwaggerOperation(Description = "This route is for Changing the user's Profile Picture")]
        [HttpPost("update/image")]
        public async Task<ActionResult<User>> UpdateProfileImage([FromForm] ImageFromUserDto imageFromUser)
        {
            var name = User.Claims.First(i => i.Type == "user_id").Value;
            var id = int.Parse(name);
            var dataForProfileUpdate = _mapper.Map<DataForProfilePicUpdateDto>(imageFromUser);
            dataForProfileUpdate.Name = name;
            var updatedProfilePicUrl = await _profileService.UploadProfileImage(dataForProfileUpdate);
            return Ok(await _repo.UpdateProfileImage(id, updatedProfilePicUrl));
        }

        [SwaggerOperation(Description = "Detailed user information. Ideal for displaying Profile info")]
        [HttpGet("view")]
        public async Task<ActionResult<UserForProfileViewDto>> View()
        {
            var id = int.Parse(User.Claims.First(i => i.Type == "user_id").Value);
            var user = await _repo.GetUser(id);
            var userForView = _mapper.Map<UserForProfileViewDto>(user);
            var institutionId = user.InstitutionId ?? default(int);
            if (user.InstitutionId > 0)
                userForView.InstitutionName = await _institution.FindName(userForView.Category, institutionId);
            return Ok(userForView);
        }

    }
}