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
        private readonly IEnvironmentService _env;
        public QRCodeGeneration(ICloudStorage cloudStorage, IConfiguration configuration, ICipherService cipher, IEnvironmentService env)
        {
            _cipher = cipher;
            _configuration = configuration;
            _cloudStorage = cloudStorage;
            _env = env;
        }

        public async Task<string> CreateQrCode(string Id)
        {
            var secretkey = _env.EncryptionQrCode;
            var encryptedId = _cipher.Encryption(secretkey, Id);
            var qrCodeImage = GenerateQrCode(encryptedId);
            var qRCodeUrl = await UploadFileAsync(qrCodeImage, Id);
            return qRCodeUrl;
        }

        // Generates Bitmap image of the encrypted excelid
        private static Bitmap GenerateQrCode(string id)
        {
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(id, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCode(qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(20);
            return qrCodeImage;
        }

        // Converts the Bitmap Image to a PNG File
        private async Task<string> UploadFileAsync(Bitmap qrCodeImage, string id)
        {
            var secretkey = _env.EncryptionFileName;
            var data = new DataForQRCodeUploadDto();
            data.Name = _cipher.Encryption(secretkey, id) + ".png";
            var bitmapBytes = BitmapToBytes(qrCodeImage);
            await using (var stream = new MemoryStream(bitmapBytes))
            {
                IFormFile Image = new FormFile(stream, 0, bitmapBytes.Length, data.Name, data.Name);
                data.Image = Image;
                var fileExtension = Path.GetExtension(data.Name);
                var fileNameForStorage = "accounts/qr-code/" + data.Name;
                await _cloudStorage.UploadFileAsync(data.Image, fileNameForStorage);
                var qRCodeUrl = _env.CloudStorageUrl + fileNameForStorage;
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