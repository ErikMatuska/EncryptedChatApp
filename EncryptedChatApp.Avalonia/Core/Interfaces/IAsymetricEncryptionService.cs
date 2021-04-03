using Microsoft.CodeAnalysis;

namespace EncryptedChatApp.Avalonia.Core.Interfaces
{
    public interface IAsymetricEncryptionService
    {
        public string Encrypt(string plainTextMessage, IPublicKeyProvider publicKeyProvider);

        public string Decrypt(string cipherTextMessage);
    }
}
