using Org.BouncyCastle.Crypto;
using System.Security.Cryptography;

namespace EncryptedChatApp.Avalonia.Core.Interfaces
{
    public interface IKeyStore : IPublicKeyProvider
    {
        public void SaveKeyPair(AsymmetricCipherKeyPair keyPair, string folderName);
        public RSAParameters GetPrivateKey(string folderName);
        public RSAParameters GetPublicKey(string folderName);
        string GetPrivateKeyString(string folderName);
    }
}
