using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Data.Interfaces;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class InstitutionRepository : IInstitutionRepository
    {
        private readonly DataContext _context;
        public InstitutionRepository(DataContext context)
        {
            this._context = context;
        }

        public async Task<College> AddCollege(string Name)
        {
            var college = new College();
            college.Name = Name;
            await _context.Colleges.AddAsync(college);
            var success = await _context.SaveChangesAsync() > 0;
            if (success) return college;
            throw new Exception("Problem saving changes");
        }

        public async Task<School> AddSchool(string Name)
        {
            var school = new School();
            school.Name = Name;
            await _context.Schools.AddAsync(school);
            var success = await _context.SaveChangesAsync() > 0;
            if (success) return school;
            throw new Exception("Problem saving changes");
        }

        public async Task<List<College>> CollegeList()
        {
            var colleges = await _context.Colleges.ToListAsync();
            return colleges;
        }

        public async Task<string> FindName(string category, int id)
        {
            var name = "";
            if (category == "college")
            {
                var college = await _context.Colleges.FindAsync(id);
                name = college.Name;
            }
            else if (category == "school")
            {
                var school = await _context.Schools.FindAsync(id);
                name = school.Name;
            }
            return name;
        }

        public async Task<List<School>> SchoolList()
        {
            var schools = await _context.Schools.ToListAsync();
            return schools;
        }
    }
}