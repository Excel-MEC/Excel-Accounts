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
    public class InstitutionController : ControllerBase
    {
        private readonly IInstitutionRepository _institution;
        public InstitutionController(IInstitutionRepository institution)
        {
            _institution = institution;
        }

        //To retrieve the list of colleges
        [HttpGet("college/list")]

        public async Task<ActionResult<List<College>>> CollegeList()
        {
            var colleges = await _institution.CollegeList();
            return Ok(new { Response = colleges });
        }

        //To add a college
        [HttpPost("college/add")]
        public async Task<ActionResult> AddCollege(DataForAddingCollegeDto data)
        {
            var college = await _institution.AddCollege(data.Name);
            return Ok(new { Response = college });
        }

        //To retrieve the list of schools
        [HttpGet("school/list")]

        public async Task<ActionResult<List<School>>> SchoolList()
        {
            var schools = await _institution.SchoolList();
            return Ok(new { Response = schools });
        }

        //To add a School
        [HttpPost("school/add")]
        public async Task<ActionResult> AddSchool(DataForAddingSchoolDto data)
        {
            var school = await _institution.AddSchool(data.Name);
            return Ok(new { Response = school });
        }
    }
}