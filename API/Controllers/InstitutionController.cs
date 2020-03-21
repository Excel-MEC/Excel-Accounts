using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Data.InstitutionRepository;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
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
        public async Task<ActionResult> AddCollege([FromForm]string Name)
        {
            var success = await _institution.AddCollege(Name);
            if (success) return Ok(new { Response = "Success" });

            throw new Exception("Problem saving changes");
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
        public async Task<ActionResult> AddSchool([FromForm]string Name)
        {
            var success = await _institution.AddSchool(Name);
            if (success) return Ok(new { Response = "Success" });

            throw new Exception("Problem saving changes");
        }
    }
}