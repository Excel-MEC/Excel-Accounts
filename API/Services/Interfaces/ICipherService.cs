namespace API.Services.Interfaces
{
    public interface ICipherService
    {
        string Encryption(string key, string textToBeEncrypted);
        string Decryption(string key, string cipherText);
    }
}