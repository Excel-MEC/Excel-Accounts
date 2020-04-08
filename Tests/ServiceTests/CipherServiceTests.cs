using System;
using API.Services;
using API.Services.Interfaces;
using Xunit;

namespace Tests.ServiceTests
{
    public class CipherServiceTests
    {
        private readonly string _key;
        private readonly string _normalString;
        private readonly string _encryptedString;
        private readonly ICipherService _cipher;
        public CipherServiceTests()
        {
            _key = "Super Secret Key";
            _normalString = "Normal";
            _encryptedString = "BH10ICbw6c3x32o0/AoSBQ==";
            _cipher = new CipherService();
        }

        [Fact]
        public void Encryption_GivenKeyAndString_ReturnsEncryptedString()
        {
            //Given
            //When
            //Then
            string result = _cipher.Encryption(_key, _normalString);
            Assert.Equal(_encryptedString, result);
        }

        [Fact]
        public void Decryption_GivenKeyAndEncryptedString_ReturnsDecryptedString()
        {
            //Given
            //When
            //Then
            string result = _cipher.Decryption(_key, _encryptedString);
            Assert.Equal(_normalString, result);
        }
    }
}