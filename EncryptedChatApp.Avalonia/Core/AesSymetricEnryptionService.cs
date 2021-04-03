using EncryptedChatApp.Avalonia.Core.Interfaces;
using EncryptedChatApp.Avalonia.Helpers;
using System;
using System.Security.Cryptography;

namespace EncryptedChatApp.Avalonia.Core
{
    public class AesSymetricEnryptionService : ISymetricEnryptionService
    {
        public string Decrypt(string cipherTextMessage, string key)
        {
            var keyBytes = Convert.FromBase64String(key);

            var aesLib = new AesBcCrypto(Helpers.CipherMode.GCM, Padding.NoPadding);

            return aesLib.Decrypt(cipherTextMessage, keyBytes);
        }

        public SymetricEnryptionResult Enrypt(string plainTextMessage)
        {
            var rnd = new RNGCryptoServiceProvider();
            var keyBytes = new byte[16];

            rnd.GetNonZeroBytes(keyBytes);

            var aes = new AesBcCrypto(Helpers.CipherMode.GCM, Padding.NoPadding);

            var encrypted = aes.Encrypt(plainTextMessage, keyBytes);

            return new SymetricEnryptionResult()
            {
                Key = Convert.ToBase64String(keyBytes),
                Content = encrypted
            };
        }
    }
}
