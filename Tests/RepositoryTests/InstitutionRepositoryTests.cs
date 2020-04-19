using System.Collections.Generic;
using System.Threading.Tasks;
using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Tests.RepositoryTests
{
    public class InstitutionRepositoryTests : IClassFixture<DbContextFixture>
    {
        private readonly DataContext _context;
        private readonly InstitutionRepository _repo;

        public InstitutionRepositoryTests(DbContextFixture dbContextFixture)
        {
            _context = dbContextFixture.Context;
            _repo = new InstitutionRepository(_context);
        }

        [Fact]
        public async Task AddCollege_GivenCollegeName_ReturnsCollegeObjectAsync()
        {
            string newCollegeName = "Add College";
            var newCollege = await _repo.AddCollege(newCollegeName);
            var collegeFromContext = await _context.Colleges.FirstOrDefaultAsync(x => x.Name == newCollege.Name);
            Assert.Equal(newCollege.Name, collegeFromContext.Name);
        }

        [Fact]
        public async Task AddSchool_GivenSchooleName_ReturnsSchoolObjectAsync()
        {
            string newSchoolName = "Add School";
            var newSchool = await _repo.AddSchool(newSchoolName);
            var schoolFromContext = await _context.Schools.FirstOrDefaultAsync(x => x.Name == newSchool.Name);
            Assert.Equal(newSchool.Name, schoolFromContext.Name);
        }

        [Fact]
        public async Task CollegeList_GivenNothing_ReturnsListOfCollegesAsync()
        {
            var Colleges = await _repo.CollegeList();
            Assert.IsType<List<College>>(Colleges);
            var collegeFromContext = await _context.Colleges.FirstOrDefaultAsync();
            var collegeFromRepo = Colleges[0];
            Assert.Equal(collegeFromContext.Name, collegeFromRepo.Name);
        }

        [Fact]
        public async Task SchoolList_GivenNothing_ReturnsListOfSchoolsAsync()
        {
            var Schools = await _repo.SchoolList();
            Assert.IsType<List<School>>(Schools);
            var schoolFromContext = await _context.Schools.FirstOrDefaultAsync();
            var schoolFromRepo = Schools[0];
            Assert.Equal(schoolFromContext.Name, schoolFromRepo.Name);
        }
    }
}