using System.Collections.Generic;
using System.Threading.Tasks;
using API.Data.Interfaces;
using API.Dtos.Institution;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers
{
    [SwaggerTag("All the routes under this controller needs Authorization header.")]
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class InstitutionController : ControllerBase
    {
        private readonly IInstitutionRepository _institution;
        public InstitutionController(IInstitutionRepository institution)
        {
            _institution = institution;
        }

        [SwaggerOperation(Description = "Raw College data stored in the database. This route is for college listing.    ")]
        [HttpGet("college/list")]
        public async Task<ActionResult<List<College>>> CollegeList()
        {
            var colleges = await _institution.CollegeList();
            return Ok(new { Response = colleges });
        }

        [SwaggerOperation(Description = "This route is to add a new college. The name is set to the new college name.")]
        [HttpPost("college/add")]
        public async Task<ActionResult> AddCollege(DataForAddingCollegeDto data)
        {
            var college = await _institution.AddCollege(data.Name);
            return Ok(new { Response = college });
        }

        [SwaggerOperation(Description = "Raw School data stored in the database. This route is for school listing.")]
        [HttpGet("school/list")]
        public async Task<ActionResult<List<School>>> SchoolList()
        {
            var schools = await _institution.SchoolList();
            return Ok(new { Response = schools });
        }

        [SwaggerOperation(Description = "This route is to add a new school. The name is set to the new school name.")]
        [HttpPost("school/add")]
        public async Task<ActionResult> AddSchool(DataForAddingSchoolDto data)
        {
            var school = await _institution.AddSchool(data.Name);
            return Ok(new { Response = school });
        }
    }
}