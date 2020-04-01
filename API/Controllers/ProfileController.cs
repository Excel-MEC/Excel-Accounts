using System;
using System.Linq;
using System.Threading.Tasks;
using API.Data.ProfileRepository;
using API.Dtos.Test;
using API.Dtos.Profile;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Data.InstitutionRepository;

namespace API.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileRepository _repo;
        private readonly IMapper _mapper;
        private readonly IInstitutionRepository _institution;
        public ProfileController(IProfileRepository repo, IMapper mapper, IInstitutionRepository institution)
        {
            _institution = institution;
            _mapper = mapper;
            _repo = repo;
        }
        //GET api/profile
        [HttpGet]
        public async Task<ActionResult<User>> Get()
        {
            int id = int.Parse(this.User.Claims.First(i => i.Type == "user_id").Value);
            var user = await _repo.GetUser(id);
            return Ok(new { Response = user });
        }

        //Edit profile 
        [HttpPut("update")]
        public async Task<ActionResult> UpdateProfile([FromForm]DataForProfileUpdateDto data)
        {
            int id = int.Parse(this.User.Claims.First(i => i.Type == "user_id").Value);
            var user = await _repo.GetUser(id);
            var success = await _repo.UpdateProfile(user, data);
            if (success) return Ok(new { Response = "Success" });
            throw new Exception("No changes were made");
        }

        //Edit profile image
        [HttpPut("update/image")]
        public async Task<ActionResult> UpdateProfileImage([FromForm]DataForFileUploadDto data)
        {
            int id = int.Parse(this.User.Claims.First(i => i.Type == "user_id").Value);
            var user = await _repo.GetUser(id);
            var success = await _repo.UpdateProfileImage(user, data);
            if (success) return Ok(new { Response = "Success" });
            throw new Exception("Problem saving changes");
        }

        //View profile
        [HttpGet("view")]
        public async Task<ActionResult<DataForProfileViewDto>> View()
        {
            int id = int.Parse(this.User.Claims.First(i => i.Type == "user_id").Value);
            var user = await _repo.GetUser(id);
            var newUser = _mapper.Map<DataForProfileViewDto>(user);
            newUser.InstitutionName = await _institution.FindName(newUser.Category, user.InstitutionId);
            return Ok(new { Response = newUser });
        }




    }
}