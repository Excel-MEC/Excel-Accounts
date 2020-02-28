using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Excel_Accounts_Backend.Data.CloudStorage;
using Excel_Accounts_Backend.Dtos.Values;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Excel_Accounts_Backend.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ICloudStorage _cloudStorage;
        private readonly IConfiguration _configuration;
        public ValuesController(ICloudStorage cloudStorage, IConfiguration configuration)
        {
            _cloudStorage = cloudStorage;
            _configuration = configuration;
        }
        // POST api/values
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { Response = "Success" });
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Post([FromForm]DataForFileUploadDto dataForFileUpload)
        {
            await UploadFile(dataForFileUpload);
            string ImageUrl = _configuration.GetValue<string>("CloudStorageUrl") + dataForFileUpload.ImageStorageName;
            return Ok(new { Response = ImageUrl });
        }

        private async Task UploadFile(DataForFileUploadDto dataForFileUpload)
        {
            string fileNameForStorage = "accounts/qr-code/" + FormFileName(dataForFileUpload.Name, dataForFileUpload.Image.FileName);
            dataForFileUpload.ImageUrl = await _cloudStorage.UploadFileAsync(dataForFileUpload.Image, fileNameForStorage);
            dataForFileUpload.ImageStorageName = fileNameForStorage;
        }

        private static string FormFileName(string title, string fileName)
        {
            var fileExtension = Path.GetExtension(fileName);
            var fileNameForStorage = $"{title}-{DateTime.Now.ToString("yyyyMMddHHmmss")}{fileExtension}";
            return fileNameForStorage;
        }
    }
}