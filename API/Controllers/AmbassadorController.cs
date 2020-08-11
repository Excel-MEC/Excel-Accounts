using System;
using System.Linq;
using System.Threading.Tasks;
using API.Data.Interfaces;
using API.Dtos.Ambassador;
using API.Models.Custom;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using API.Extensions.CustomExceptions;

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
            this._mapper = mapper;
            this._repo = repo;
        }

        [SwaggerOperation(Description = "Sign up for Ambassador")]
        [HttpGet("signup")]
        public async Task<ActionResult> SignUpForAmbassador()
        {
            int id = int.Parse(this.User.Claims.First(i => i.Type == "user_id").Value);
            var success = await _repo.SignUpForAmbassador(id);
            if (!success) throw new Exception("Problem saving changes");
            return Ok(new OkResponse { Response = "Success" });
            
        }

        [SwaggerOperation(Description = "Ambassador Profile view")]
        [HttpGet]
        public async Task<ActionResult<AmbassadorProfileDto>> View()
        {
            int id = int.Parse(this.User.Claims.First(i => i.Type == "user_id").Value);
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
        public async Task<ActionResult> ApplyReferralCode(DataForApplyingReferralCodeDto data)
        {
            int id = int.Parse(this.User.Claims.First(i => i.Type == "user_id").Value);
            var success = await _repo.ApplyReferralCode(id, data.referralCode);    
            if (success) return Ok(new OkResponse { Response = "Success" });
            throw new Exception("Problem saving changes");
        }

        [SwaggerOperation(Description = "List of Referred Users")]
        [HttpGet("userlist")]
        public async Task<ActionResult<List<UserViewDto>>> ListOfReferredUsers()
        {
            int id = int.Parse(this.User.Claims.First(i => i.Type == "user_id").Value);
            var referredUsers = await _repo.ListOfReferredUsers(id);    
            return Ok(referredUsers);
        }

    }
}