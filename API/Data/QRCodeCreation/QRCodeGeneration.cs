using System;
using System.IO;
using System.Threading.Tasks;
using API.Data.CloudStorage;
using API.Dtos.Test;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Drawing;
using QRCoder;
using Microsoft.AspNetCore.Http;

namespace API.Data.QRCodeCreation
{
    public class QRCodeGeneration : IQRCodeGeneration
    {
        private readonly ICloudStorage _cloudStorage;
        private readonly IConfiguration _configuration;
        public QRCodeGeneration(ICloudStorage cloudStorage, IConfiguration configuration)
        {
            _configuration = configuration;
            _cloudStorage = cloudStorage;
        }

        public async Task<string> CreateQrCode([FromForm]string ExcelId)
        {
            DataForFileUploadDto qrCodeDto = new DataForFileUploadDto();
            qrCodeDto.Name = ExcelId;
            Bitmap qrCodeImage = GenerateQrCode(qrCodeDto);
            BitmapToImageFile(qrCodeImage, qrCodeDto);

            await UploadFile(qrCodeDto);
            string ImageUrl = _configuration.GetValue<string>("CloudStorageUrl") + qrCodeDto.ImageStorageName;
            return ImageUrl;
        }

        // Generates Bitmap image of the string
        private static Bitmap GenerateQrCode(DataForFileUploadDto qrCodeDto)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrCodeDto.Name, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            return qrCodeImage;
        }

        // Converts the Bitmap Image to a PNG File
        private static void BitmapToImageFile(Bitmap qrCodeImage, DataForFileUploadDto qrCodeDto)
        {
            byte[] bitmapBytes = BitmapToBytes(qrCodeImage);
            var stream = new MemoryStream(bitmapBytes);
            IFormFile Image = new FormFile(stream, 0, bitmapBytes.Length, qrCodeDto.Name, qrCodeDto.Name + ".png");
            qrCodeDto.Image = Image;
        }

        // Converts the Bitmap to byte array
        private static byte[] BitmapToBytes(Bitmap img)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
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