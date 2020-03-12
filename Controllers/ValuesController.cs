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
using Excel_Accounts_Backend.Security;
using System.Drawing;
using System.Drawing.Imaging;
using QRCoder;
using Microsoft.AspNetCore.Http;

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

        [HttpPost("qrcode")]
        public async Task<IActionResult> CreateQrCode([FromBody] QRCodeFileUploadDto qRCodeFileUpload)
        {
            string key = _configuration.GetSection("AppSettings:Token").Value;
            string encryptedString = EncodingDecoding.EncryptString(key, qRCodeFileUpload.Id);
            var decryptedString = EncodingDecoding.DecryptString(key, encryptedString);
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(encryptedString, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            byte[] bitmapBytes = BitmapToBytes(qrCodeImage); //Convert bitmap into a byte array
            using (Image img = Image.FromStream(new MemoryStream(bitmapBytes)))
            {
                img.Save($"{encryptedString}.jpg", ImageFormat.Jpeg);
                // qRCodeFileUpload.Image = img;
            }
             await UploadQRCode(encryptedString, qRCodeFileUpload);
            // string ImageUrl = _configuration.GetValue<string>("CloudStorageUrl") + qRCodeFileUpload.ImageStorageName;
            // return Ok(new { Response = ImageUrl});
            return Ok(new { Response = "Success"});

            
        }

        private static byte[] BitmapToBytes(Bitmap img)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }
        private async Task UploadQRCode(string encryptedString, QRCodeFileUploadDto qRCodeFileUpload)
        {
            string fileName = $"{encryptedString}-{DateTime.Now.ToString("yyyyMMddHHmmss")}" + ".jpg";
            string fileNameForStorage = "accounts/qr-code/" + fileName;
            qRCodeFileUpload.ImageUrl = await _cloudStorage.UploadFileAsync(qRCodeFileUpload.Image, fileNameForStorage);
            qRCodeFileUpload.ImageStorageName = fileNameForStorage;
        }
    }
}