using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Excel_Accounts_Backend.Data.ProfileRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Excel_Accounts_Backend.Controllers
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
        // POST api/values
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            int id = int.Parse(this.User.Claims.First(i => i.Type == "user_id").Value);
            var user = await _repo.GetUser(id);
            return Ok(user);
        }
    }
}