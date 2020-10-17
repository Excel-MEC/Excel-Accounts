using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data.Interfaces;
using API.Dtos.Admin;
using API.Dtos.Profile;
using API.Models;
using API.Models.Custom;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers
{
    [SwaggerTag("All the routes under this controller require privileges > User.")]
    [Route("[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class AdminController : ControllerBase
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IInstitutionRepository _institutionRepository;
        private readonly IMapper _mapper;

        public AdminController(IProfileRepository profileRepository, IInstitutionRepository institutionRepository,
            IMapper mapper)
        {
            _profileRepository = profileRepository;
            _institutionRepository = institutionRepository;
            _mapper = mapper;
        }
        
        [SwaggerOperation(Description =
            "Full data of all users. Route accessible only to the roles: Admin, Core, Editor, Staff")]
        [HttpGet("users")]
        [Authorize(Roles = "Admin, Core, Editor, Staff")]
        public async Task<ActionResult<OkResponseWithPagination<User>>> GetAllUsers([FromQuery] QueryParametersForGetAllUsers parameters)
        {
            var users = await _profileRepository.GetAllUser(parameters);
            var metadata = new Pagination()
            {
                TotalCount = users.TotalCount,
                PageSize = users.PageSize,
                CurrentPage = users.CurrentPage,
                TotalPages = users.TotalPages,
                HasNext = users.HasNext,
                HasPrevious = users.HasPrevious
            };
            return Ok(new OkResponseWithPagination<User>() {Data = users, Pagination = metadata});
        }
        
        [SwaggerOperation(Description = "Retrieves the List of staffs")]
        [HttpGet("staffs")]
        [Authorize(Roles = "Admin, Core, Editor, Staff")]
        public async Task<ActionResult<List<User>>> GetStaffs()
        {
            var users = await _profileRepository.GetStaffs();
            return Ok(users);
        }

        [SwaggerOperation(Description =
            "Basic user data corresponding to the list of the excelIds posted. Route accessible only to the roles: Admin, Core, Editor, Staff")]
        [Authorize(Policy = "ServiceAccount")]
        [HttpPost("users")]
        public async Task<ActionResult<List<User>>> GetUsers(List<int> excelIds)
        {
            var users = await _profileRepository.GetUserList(excelIds);
            return Ok(users);
        }

        [SwaggerOperation(Description =
            " Route to change the role. Route accessible only to the roles: Admin, Core, Editor")]
        [Authorize(Roles = "Admin, Core, Editor")]
        [HttpPut("users/permission")]
        public async Task<ActionResult<User>> ChangeRole(DataForChangingRoleDto dataForChangingRoleDto)
        {
            var newroles = dataForChangingRoleDto.Role.Split(",").Select(x => x.Trim()).ToList();
            var oldRole = await _profileRepository.GetRole(dataForChangingRoleDto.Id);
            var oldRoles = oldRole.Split(",").Select(x => x.Trim()).ToList();
            if (CheckPermission(newroles) && CheckPermission(oldRoles))
            {
                return Ok(await _profileRepository.ChangeRole(dataForChangingRoleDto));
            }

            throw new UnauthorizedAccessException("You don't have the permission to do that");
        }
        
        [SwaggerOperation(Description =
            " Route to change the payment status of an user. Route accessible only to the roles: Admin, Core, Editor")]
        [Authorize(Roles = "Admin, Core, Editor, Accountant")]
        [HttpPut("users/payment")]
        public async Task<ActionResult<User>> ChangePaymentStatus(DataForChangingPaymentStatusDto data)
        {
            return Ok(await _profileRepository.UpdatePaymentStatus(data));
        }

        [SwaggerOperation(Description =
            "Full data of the user corresponding to the user Id. Route accessible only to the roles: Admin, Core, Editor, Staff")]
        [Authorize(Roles = "Admin, Core, Editor,  Staff")]
        [HttpGet("users/{id}")]
        public async Task<ActionResult<UserForProfileViewDto>> GetUserData(int id)
        {
            var user = await _profileRepository.GetUser(id);
            var userForView = _mapper.Map<UserForProfileViewDto>(user);
            var institutionId = user.InstitutionId ?? default(int);
            if (user.InstitutionId > 0)
                userForView.InstitutionName =
                    await _institutionRepository.FindName(userForView.Category, institutionId);
            return Ok(userForView);
        }
        
        [SwaggerOperation(Description =
            "Removes user account. Only admins can access this route.")]
        [Authorize(Roles = "Admin")]
        [HttpDelete("users/{id}")]
        public async Task<ActionResult<User>> RemoveUser(int id)
        {
            return Ok( await _profileRepository.RemoveUser(id));
        }
        
        private bool CheckPermission(List<string> roles)
        {
            if (roles.Contains(Roles.Admin) || roles.Contains(Roles.Core) || roles.Contains(Roles.Editor))
            {
                if (!User.IsInRole(Roles.Admin))
                    return false;
            }

            if (roles.Contains(Roles.Staff))
            {
                if (!(User.IsInRole(Roles.Admin) || User.IsInRole(Roles.Core) ||
                    User.IsInRole(Roles.Editor)))
                    return false;
            }

            return true;
        }
    }
}