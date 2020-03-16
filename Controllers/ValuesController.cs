using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Excel_Accounts_Backend.Data.CloudStorage;
using Excel_Accounts_Backend.Data.QRCodeCreation;
using Excel_Accounts_Backend.Dtos.Values;
using Excel_Accounts_Backend.Models;
using Excel_Accounts_Backend.Data.AuthRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Excel_Accounts_Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace Excel_Accounts_Backend.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ICloudStorage _cloudStorage;
        private readonly IConfiguration _configuration;
        private readonly IAuthRepository _authRepository;
        private readonly IQRCodeGeneration _qRCodeGeneration;
        private readonly DataContext _context;
        public ValuesController(ICloudStorage cloudStorage, IConfiguration configuration, IAuthRepository authRepository, IQRCodeGeneration qRCodeGeneration, DataContext context)
        {
            _context = context;
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
        //To generate the qrcode
        [HttpPost("qrcode")]

        public async Task<string> CreateQrCode([FromForm]string ExcelId)
        {
            string qRCodeUrl = await _qRCodeGeneration.CreateQrCode(ExcelId);
            return qRCodeUrl;

        }

        //To retrieve the list of colleges
        [HttpGet("college/list")]

        public async Task<ActionResult<List<College>>> List()
        {
            var colleges = await _context.Colleges.ToListAsync();
            return Ok(new { Response = colleges });
        }

        //To retrieve a college
        [HttpPost("college")]
        public async Task<ActionResult> College([FromForm]string Name)
        {

            var college = new College();
            college.Name = Name;
            await _context.Colleges.AddAsync(college);
            var success = await _context.SaveChangesAsync() > 0;

            if (success) return Ok(new { Response = "Success" });

            throw new Exception("Problem saving changes");
        }
    }
}