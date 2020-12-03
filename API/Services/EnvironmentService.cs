using System;
using System.Text;
using API.Services.Interfaces;

namespace API.Services
{
    public class EnvironmentService: IEnvironmentService
    {
        public string PostgresDb { get; }
        public string GoogleCloudStorageBucket { get; }
        public string CloudStorageUrl { get; }
        public string GoogleClientId { get; set; }
        public string GoogleClientSecret { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string Issuer { get; set; }
        public string EncryptionQrCode { get; set; }
        public string EncryptionFileName { get; set; }
        public string GoogleApi { get; set; }
        public string ApiPrefix { get; }
        public string ServiceKey { get; }
        public string GoogleCredential { get; }
        
        public EnvironmentService()
                {
                     PostgresDb = Environment.GetEnvironmentVariable("POSTGRES_DB");
                     GoogleCloudStorageBucket = Environment.GetEnvironmentVariable("GOOGLE_CLOUD_STORAGE_BUCKET");
                     CloudStorageUrl = Environment.GetEnvironmentVariable("CLOUD_STORAGE_URL");
                     GoogleClientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
                     GoogleClientSecret = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET");
                     AccessToken = Environment.GetEnvironmentVariable("ACCESS_TOKEN");
                     RefreshToken = Environment.GetEnvironmentVariable("REFRESH_TOKEN");
                     Issuer = Environment.GetEnvironmentVariable("ISSUER");
                     EncryptionQrCode = Environment.GetEnvironmentVariable("ENCRYPTION_QRCODE");
                     EncryptionFileName = Environment.GetEnvironmentVariable("ENCRYPTION_FILENAME");
                     GoogleApi = Environment.GetEnvironmentVariable("GOOGLEAPI");
                     ApiPrefix = Environment.GetEnvironmentVariable("API_PREFIX");
                     ServiceKey = Environment.GetEnvironmentVariable("SERVICE_KEY");
                     GoogleCredential = Encoding.UTF8.GetString(Convert.FromBase64String(Environment.GetEnvironmentVariable("GOOGLE_CREDENTIAL")!)) ;
                }
    }
}