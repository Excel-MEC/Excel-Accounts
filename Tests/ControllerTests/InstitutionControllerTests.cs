using System.Collections.Generic;
using System.Threading.Tasks;
using API.Controllers;
using API.Data.Interfaces;
using API.Dtos.Institution;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Tests.ControllerTests
{
    public class InstitutionControllerTests
    {
        private readonly Moq.Mock<IInstitutionRepository> _repo;
        private readonly InstitutionController _controller;
        public InstitutionControllerTests()
        {
            _repo = new Mock<IInstitutionRepository>();
            _controller = new InstitutionController(_repo.Object);
        }

        [Fact]
        public async Task CollegeList_GivenNothing_ReturnsListOfCollegeAsync()
        {
            //Given
            var collegeList = Mock.Of<List<College>>();
            //When
            _repo.Setup(x => x.CollegeList()).ReturnsAsync(collegeList);
            //Then
            var response = await _controller.CollegeList();
            if (response.Result is OkObjectResult okObjectResult)
            {
                var responseData = okObjectResult.Value as List<College>;
                Assert.IsAssignableFrom<List<College>>(responseData);
            }
        }

        [Fact]
        public void AddCollege_GivenCollegeName_ReturnsCollegeInstance()
        {
            //Given
            var dataForAddingCollege = Mock.Of<DataForAddingCollegeDto>();
            dataForAddingCollege.Name = "New College";
            var newCollege = Mock.Of<College>();
            newCollege.Name = dataForAddingCollege.Name;
            //When
            _repo.Setup(x => x.AddCollege(dataForAddingCollege.Name)).ReturnsAsync(newCollege);
            //Then
            var response = _controller.AddCollege(dataForAddingCollege);
            OkObjectResult okObjectResult = response.Result as OkObjectResult;
            var responseData = okObjectResult.Value as College;
            var collegeFromController = Assert.IsAssignableFrom<College>(responseData);
            Assert.Equal(dataForAddingCollege.Name, collegeFromController.Name);
        }

        [Fact]
        public async Task SchoolList_GivenNothing_ReturnsListOfSchoolAsync()
        {
            //Given
            var schoolList = Mock.Of<List<School>>();
            //When
            _repo.Setup(x => x.SchoolList()).ReturnsAsync(schoolList);
            //Then
            var response = await _controller.SchoolList();
            OkObjectResult okObjectResult = response.Result as OkObjectResult;
            var responseData = okObjectResult.Value as List<School>;
            Assert.IsAssignableFrom<List<School>>(responseData);
        }

        [Fact]
        public void AddSchool_GivenSchooleName_ReturnsSchoolInstance()
        {
            //Given
            var dataForAddingSchool = Mock.Of<DataForAddingSchoolDto>();
            dataForAddingSchool.Name = "New School";
            var newSchool = Mock.Of<School>();
            newSchool.Name = dataForAddingSchool.Name;
            //When
            _repo.Setup(x => x.AddSchool(dataForAddingSchool.Name)).ReturnsAsync(newSchool);
            //Then
            var response = _controller.AddSchool(dataForAddingSchool);
            OkObjectResult okObjectResult = response.Result as OkObjectResult;
            var responseData = okObjectResult.Value as School;
            var schoolFromController = Assert.IsAssignableFrom<School>(responseData);
            Assert.Equal(dataForAddingSchool.Name, schoolFromController.Name);
        }
    }
}