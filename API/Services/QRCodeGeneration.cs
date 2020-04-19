using System.IO;
using System.Threading.Tasks;
using API.Dtos.QRCodeGeneration;
using Microsoft.Extensions.Configuration;
using System.Drawing;
using QRCoder;
using Microsoft.AspNetCore.Http;
using API.Services.Interfaces;
using System;

namespace API.Services
{
    public class QRCodeGeneration : IQRCodeGeneration
    {
        private readonly ICloudStorage _cloudStorage;
        private readonly IConfiguration _configuration;
        private readonly ICipherService _cipher;
        public QRCodeGeneration(ICloudStorage cloudStorage, IConfiguration configuration, ICipherService cipher)
        {
            _cipher = cipher;
            _configuration = configuration;
            _cloudStorage = cloudStorage;
        }

        public async Task<string> CreateQrCode(string Id)
        {
            string secretkey = _configuration.GetSection("AppSettings:Encryption:Qrcode").Value;
            var encryptedId = _cipher.Encryption(secretkey, Id);
            Bitmap qrCodeImage = GenerateQrCode(encryptedId);
            string qRCodeUrl = await UploadFileAsync(qrCodeImage, Id);
            return qRCodeUrl;
        }

        // Generates Bitmap image of the encrypted excelid
        private static Bitmap GenerateQrCode(string id)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(id, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            return qrCodeImage;
        }

        // Converts the Bitmap Image to a PNG File
        private async Task<string> UploadFileAsync(Bitmap qrCodeImage, string id)
        {
            string secretkey = _configuration.GetSection("AppSettings:Encryption:Filename").Value;
            DataForQRCodeUploadDto data = new DataForQRCodeUploadDto();
            data.Name = _cipher.Encryption(secretkey, id) + ".png";
            byte[] bitmapBytes = BitmapToBytes(qrCodeImage);
            using (MemoryStream stream = new MemoryStream(bitmapBytes))
            {
                IFormFile Image = new FormFile(stream, 0, bitmapBytes.Length, data.Name, data.Name);
                data.Image = Image;
                var fileExtension = Path.GetExtension(data.Name);
                string fileNameForStorage = "accounts/qr-code/" + data.Name;
                await _cloudStorage.UploadFileAsync(data.Image, fileNameForStorage);
                string qRCodeUrl = _configuration.GetValue<string>("CloudStorageUrl") + fileNameForStorage;
                return qRCodeUrl;
            }
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
    }
}