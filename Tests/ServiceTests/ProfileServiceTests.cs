using API.Data.Interfaces;
using API.Dtos.Profile;
using API.Services;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Tests.ServiceTests
{
    public class ProfileServiceTests
    {
        private readonly Mock<ICloudStorage> _cloudStorage;
        private readonly Mock<IConfiguration> _configuration;
        private readonly IProfileService _profileService;
        public ProfileServiceTests()
        {
            _cloudStorage = new Mock<ICloudStorage>();
            _configuration = new Mock<IConfiguration>();
            _profileService = new ProfileService(_cloudStorage.Object, _configuration.Object);
        }

        [Fact]
        public async System.Threading.Tasks.Task UploadFile_GivenDataForProfilePicUpdate_ReturnsNewUrlAsync()
        {
            //Given
            DataForProfilePicUpdateDto data = Mock.Of<DataForProfilePicUpdateDto>();
            data.Name = "imageName";
            var mockImage = new Mock<IFormFile>();
            mockImage.Setup(x => x.FileName).Returns("mockfile.png");
            data.Image = mockImage.Object;
            var returnUrl = "urlaccounts/profile/imageName.png";
            //When
            _configuration.Setup(x => x.GetSection("CloudStorageUrl").Value).Returns("url");
            //Then
            var response = await _profileService.UploadProfileImage(data);
            Assert.Equal(returnUrl, response);
        }
    }
}