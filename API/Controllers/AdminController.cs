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
    [Authorize(Roles = "Admin, Core, Editor, Staff")]
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
        
        public async Task<ActionResult<List<User>>> GetAllUsers([FromQuery] QueryParametersForGetAllUsers parameters)
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
        public async Task<ActionResult<List<User>>> GetStaffs()
        {
            var users = await _profileRepository.GetStaffs();
            return Ok(users);
        }

        [SwaggerOperation(Description =
            "Basic user data corresponding to the list of the excelIds posted. Route accessible only to the roles: Admin, Core, Editor, Staff")]
        [HttpPost("users")]
        public async Task<ActionResult<List<User>>> GetUsers(List<int> excelIds)
        {
            var users = await _profileRepository.GetUserList(excelIds);
            return Ok(users);
        }

        [SwaggerOperation(Description =
            " Route to change the role. Route accessible only to the roles: Admin, Core, Editor")]
        [Authorize(Roles = "Admin, Core, Editor")]
        [HttpPost("users/permission")]
        public async Task<ActionResult<List<User>>> ChangeRole(DataForChangingRoleDto dataForChangingRoleDto)
        {
            var newroles = dataForChangingRoleDto.Role.Split(",").Select(x => x.Trim()).ToList();
            var oldRole = await _profileRepository.GetRole(dataForChangingRoleDto.Id);
            var oldRoles = oldRole.Split(",").Select(x => x.Trim()).ToList();
            if (CheckPermission(newroles) && CheckPermission(oldRoles))
            {
                var success = await _profileRepository.ChangeRole(dataForChangingRoleDto);
                return Ok(new OkResponse() {Response = "Success"});
            }

            throw new UnauthorizedAccessException("You dont have the permission to do that");
        }

        [SwaggerOperation(Description =
            "Full data of the user corresponding to the user Id. Route accessible only to the roles: Admin, Core, Editor, Staff")]
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
        private bool CheckPermission(List<string> roles)
        {
            if (roles.Contains(Roles.Admin) || roles.Contains(Roles.Core) || roles.Contains(Roles.Editor))
            {
                if (!this.User.IsInRole(Roles.Admin))
                    return false;
            }

            if (roles.Contains(Roles.Staff))
            {
                if (!(this.User.IsInRole(Roles.Admin) || this.User.IsInRole(Roles.Core) ||
                    this.User.IsInRole(Roles.Editor)))
                    return false;
            }

            return true;
        }
    }
}