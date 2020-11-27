namespace API.Services.Interfaces
{
    public interface IEnvironmentService
    {
        public string PostgresDb { get; }
        public string GoogleCloudStorageBucket { get; }
        public string CloudStorageUrl{ get; }
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
    }
}