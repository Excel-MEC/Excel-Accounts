using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using API.Data.Interfaces;
using API.Dtos.Test;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace API.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ICloudStorage _cloudStorage;
        private readonly IConfiguration _configuration;
        private readonly IAuthRepository _authRepository;
        private readonly IQRCodeGeneration _qRCodeGeneration;
        private readonly ICipherService _cipher;
        public TestController(ICloudStorage cloudStorage, IConfiguration configuration, IAuthRepository authRepository, IQRCodeGeneration qRCodeGeneration, ICipherService cipher)
        {
            _cipher = cipher;
            _qRCodeGeneration = qRCodeGeneration;
            _authRepository = authRepository;
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
        public async Task<IActionResult> Post(DataForFileUploadDto dataForFileUpload)
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
        //To generate the qrcode
        [HttpPost("qrcode")]

        public async Task<string> CreateQrCode(string ExcelId)
        {
            string qRCodeUrl = await _qRCodeGeneration.CreateQrCode(ExcelId);
            return qRCodeUrl;

        }

        //To encrypt a text
        [HttpPost("cipher")]
        public string Cipher(string ExcelId)
        {
            string secretkey = _configuration.GetSection("AppSettings:Encryption:Qrcode").Value;
            string cipherText = _cipher.Encryption(secretkey, ExcelId);
            string id = _cipher.Decryption(secretkey, cipherText);
            return secretkey + "\t" + cipherText + "\t" + id;
        }
    }   
}