using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Data.Interfaces;
using API.Extensions.CustomExceptions;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class InstitutionRepository : IInstitutionRepository
    {
        private readonly DataContext _context;
        public InstitutionRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<College> AddCollege(string name)
        {
            var college = new College {Name = name};
            await _context.Colleges.AddAsync(college);
            var success = await _context.SaveChangesAsync() > 0;
            if (success) return college;
            throw new Exception("Problem saving changes");
        }

        public async Task<School> AddSchool(string name)
        {
            var school = new School {Name = name};
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
            switch (category)
            {
                case "college":
                {
                    var college = await _context.Colleges.FindAsync(id);
                    return college.Name;
                }
                case "school":
                {
                    var school = await _context.Schools.FindAsync(id);
                    return school.Name;
                }
                default:
                    throw new DataInvalidException("Invalid Category.");
            }
        }

        public async Task<List<School>> SchoolList()
        {
            var schools = await _context.Schools.ToListAsync();
            return schools;
        }
    }
}