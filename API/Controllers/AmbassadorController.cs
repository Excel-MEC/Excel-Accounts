using System.Linq;
using System.Threading.Tasks;
using API.Data.Interfaces;
using API.Dtos.Ambassador;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using API.Models;

namespace API.Controllers
{
    [SwaggerTag("All the routes under this controller needs Authorization header.")]
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class AmbassadorController : ControllerBase
    {
        private readonly IAmbassadorRepository _repo;
        private readonly IMapper _mapper;
        public AmbassadorController(IAmbassadorRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }

        [SwaggerOperation(Description = "Sign up for Ambassador")]
        [HttpGet("signup")]
        public async Task<ActionResult> SignUpForAmbassador()
        {
            var id = int.Parse(User.Claims.First(i => i.Type == "user_id").Value);
            return Ok(await _repo.SignUpForAmbassador(id));
        }

        [SwaggerOperation(Description = "Ambassador Profile view")]
        [HttpGet]
        public async Task<ActionResult<AmbassadorProfileDto>> View()
        {
            var id = int.Parse(this.User.Claims.First(i => i.Type == "user_id").Value);
            var ambassador = await _repo.GetAmbassador(id);
            return Ok(ambassador);
        }


        [SwaggerOperation(Description = "List of Ambassadors")]
        [HttpGet("list")]
        public async Task<ActionResult<List<AmbassadorListViewDto>>> ListOfAmbassadors()
        {
            var ambassadors = await _repo.ListOfAmbassadors();    
            return Ok(ambassadors);
        }

        [SwaggerOperation(Description = "Apply ReferralCode")]
        [HttpPost("referral")]
        public async Task<ActionResult<User>> ApplyReferralCode(DataForApplyingReferralCodeDto data)
        {
            var id = int.Parse(User.Claims.First(i => i.Type == "user_id").Value);
            return Ok( await _repo.ApplyReferralCode(id, data.referralCode) );
        }

        [SwaggerOperation(Description = "List of Referred Users")]
        [HttpGet("userlist")]
        public async Task<ActionResult<List<UserViewDto>>> ListOfReferredUsers()
        {
            var id = int.Parse(this.User.Claims.First(i => i.Type == "user_id").Value);
            var referredUsers = await _repo.ListOfReferredUsers(id);    
            return Ok(referredUsers);
        }

    }
}