using System;
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
            int id = int.Parse(this.User.Claims.First(i => i.Type == "user_id").Value);
            var user = await _repo.GetUser(id);
            return Ok(new { Response = user });
        }

        [SwaggerOperation(
            Description = "This route is for Updating User Profile. For category, use (college/school/professional). For the institution id, obtain the list of institution from the backend and send the id of the institution the user select.\n If The user wants to add a new institution, Set the instution id to 0  and set the institution name as the Name of new institution. Institution name and id are not applicable for professional category"
        )]
        [HttpPost("update")]
        public async Task<ActionResult> UpdateProfile(UserForProfileUpdateDto data)
        {
            int id = int.Parse(this.User.Claims.First(i => i.Type == "user_id").Value);
            var success = await _repo.UpdateProfile(id, data);
            if (success) return Ok(new { Response = "Success" });
            throw new Exception("No changes were made");
        }

        [SwaggerOperation(Description = "This route is for Changing the user's Profile Pic")]
        [HttpPost("update/image")]
        public async Task<ActionResult> UpdateProfileImage([FromForm]ImageFromUserDto imageFromUser)
        {
            string name = this.User.Claims.First(i => i.Type == "user_id").Value;
            int id = int.Parse(name);
            var dataForProfileUpdate = _mapper.Map<DataForProfilePicUpdateDto>(imageFromUser);
            dataForProfileUpdate.Name = name;
            string updatedProfilePicUrl = await _profileService.UploadProfileImage(dataForProfileUpdate);
            bool success = await _repo.UpdateProfileImage(id, updatedProfilePicUrl);
            if (success) return Ok(new { Response = "Success" });
            throw new Exception("Problem saving changes");
        }

        [SwaggerOperation(Description = "Detailed user information. Ideal for displaying Profile info")]
        [HttpGet("view")]
        public async Task<ActionResult<UserForProfileViewDto>> View()
        {
            int id = int.Parse(this.User.Claims.First(i => i.Type == "user_id").Value);
            var user = await _repo.GetUser(id);
            var userForView = _mapper.Map<UserForProfileViewDto>(user);
            userForView.InstitutionName = await _institution.FindName(userForView.Category, user.InstitutionId);
            return Ok(new { Response = userForView });
        }
    }
}