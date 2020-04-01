namespace API.Data.CipherRepository
{
    public interface ICipherRepository
    {
        string Encryption(string key,string textToBeEncrypted);
        string Decryption(string key, string cipherText);    
    }
}