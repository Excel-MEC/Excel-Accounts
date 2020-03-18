using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Excel_Accounts_Backend.Data;
using Excel_Accounts_Backend.Dtos.Institution;
using Excel_Accounts_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Excel_Accounts_Backend.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class InstitutionController : ControllerBase
    {
        private readonly DataContext _context;
        public InstitutionController(DataContext context)
        {
            _context = context;
        }

        //To retrieve the list of colleges
        [HttpGet("college/list")]

        public async Task<ActionResult<List<College>>> CollegeList()
        {
            var colleges = await _context.Colleges.ToListAsync();
            return Ok(new { Response = colleges });
        }

        //To add a college
        [HttpPost("college")]
        public async Task<ActionResult> AddCollege([FromForm]string Name)
        {

            var college = new College();
            college.Name = Name;
            await _context.Colleges.AddAsync(college);
            var success = await _context.SaveChangesAsync() > 0;

            if (success) return Ok(new { Response = "Success" });

            throw new Exception("Problem saving changes");
        }

        //To retrieve the list of schools
        [HttpGet("school/list")]

        public async Task<ActionResult<List<School>>> SchoolList()
        {
            var schools = await _context.Schools.ToListAsync();
            return Ok(new { Response = schools });
        }

        //To add a School
        [HttpPost("school")]
        public async Task<ActionResult> AddSchool([FromForm]SchoolDto school)
        {

            var newschool = new School();
            newschool.Name = school.Name;
            await _context.Schools.AddAsync(newschool);
            var success = await _context.SaveChangesAsync() > 0;

            if (success) return Ok(new { Response = "Success" });

            throw new Exception("Problem saving changes");
        }
    }
}