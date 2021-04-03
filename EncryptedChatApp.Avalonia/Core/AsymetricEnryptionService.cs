using EncryptedChatApp.Avalonia.Core.Interfaces;
using EncryptedChatApp.Avalonia.ViewModels;
using System;
using System.Security.Cryptography;
using System.Text;

namespace EncryptedChatApp.Avalonia.Core
{
    public class AsymetricEnryptionService : IAsymetricEncryptionService
    {
        private readonly IKeyStore keyStore;
        private readonly IFolderProvider folderProvider;

        public AsymetricEnryptionService(IKeyStore keyStore, IFolderProvider folderProvider)
        {
            this.keyStore = keyStore;
            this.folderProvider = folderProvider;
        }
        public string Decrypt(string cipherTextMessage)
        {
            var privateKey = keyStore.GetPrivateKey(folderProvider.GetFolderName());

            var csp = new RSACryptoServiceProvider(2048);
            csp.ImportParameters(privateKey);

            var bytesCipherText = Convert.FromBase64String(cipherTextMessage);
            var bytesPlainText = csp.Decrypt(bytesCipherText, false);

            return Encoding.Unicode.GetString(bytesPlainText);
        }

        public string Encrypt(string plainTextMessage, IPublicKeyProvider publicKeyProvider)
        {
            var publicKey = AsymetricKeyParameterExtensions
                                .GetPublicKeyFromString(publicKeyProvider
                                    .GetPublicKeyString(folderProvider.GetFolderName()));

            var csp = new RSACryptoServiceProvider(2048);
            csp.ImportParameters(publicKey);

            var bytesPlainText = Encoding.Unicode.GetBytes(plainTextMessage);
            var bytesCipherText = csp.Encrypt(bytesPlainText, false);

            return Convert.ToBase64String(bytesCipherText);
        }
    }
}
