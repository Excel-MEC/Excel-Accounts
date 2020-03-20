using System;
using System.Linq;
using System.Threading.Tasks;
using API.Data.ProfileRepository;
using API.Dtos.Profile;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileRepository _repo;
        public ProfileController(IProfileRepository repo)
        {
            _repo = repo;
        }
        //GET api/profile
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            int id = int.Parse(this.User.Claims.First(i => i.Type == "user_id").Value);
            var user = await _repo.GetUser(id);
            return Ok( new { Response = user});
        }

        [HttpPost("update")]
        public async Task<ActionResult> UpdateProfile([FromForm]DataForProfileUpdateDto data)
        {
            int id = int.Parse(this.User.Claims.First(i => i.Type == "user_id").Value);
            var user = await _repo.GetUser(id);
            var success = await _repo.UpdateProfile(user,data);
            if(success)    return Ok( new {Response = "Success"});
            throw new Exception("Problem saving changes");
        }
    }
}