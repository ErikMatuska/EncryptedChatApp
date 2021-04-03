using EncryptedChatApp.Avalonia.Core.Interfaces;
using Org.BouncyCastle.Crypto;
using System;
using System.IO;
using System.Security.Cryptography;

namespace EncryptedChatApp.Avalonia.Core
{
    public class AppDataKeyStore : IKeyStore
    {
        private readonly string AppDataFolder =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ChatEncryptedApp");

        public RSAParameters GetPrivateKey(string folderName)
        {
            var privateKey = GetPrivateKeyString(folderName);

            var rsa = RSA.Create();

            rsa.ImportFromPem(privateKey.ToCharArray());

            return rsa.ExportParameters(true);
        }

        public string GetPrivateKeyString(string folderName)
        {
            var basePath = Path.Combine(AppDataFolder, $"{folderName}.data");

            if (!Directory.Exists(basePath))
                throw new Exception("Key store does not exists!");

            var privateKeyPath = Path.Combine(basePath, "private.pem");
            var privateKey = File.ReadAllText(privateKeyPath);

            return privateKey;
        }

        public RSAParameters GetPublicKey(string folderName)
        {
            var publicKey = GetPublicKeyString(folderName);

            var rsa = RSA.Create();

            rsa.ImportFromPem(publicKey.ToCharArray());

            return rsa.ExportParameters(false);
        }

        public string GetPublicKeyString(string folderName)
        {
            var basePath = Path.Combine(AppDataFolder, $"{folderName}.data");

            if (!Directory.Exists(basePath))
                throw new Exception("Key store does not exists!");

            var publicKeyPath = Path.Combine(basePath, "public.pem");
            var publicKey = File.ReadAllText(publicKeyPath);

            return publicKey;
        }

        public void SaveKeyPair(AsymmetricCipherKeyPair keyPair, string folderName)
        {
            var publicKey = AsymetricKeyParameterExtensions.GetKeyStringFromParameter(keyPair.Public);
            var privateKey = AsymetricKeyParameterExtensions.GetKeyStringFromParameter(keyPair.Private);

            var basePath = Path.Combine(AppDataFolder, $"{folderName}.data");

            Directory.CreateDirectory(basePath);

            var publicKeyPath = Path.Combine(basePath, "public.pem");
            var privateKeyPath = Path.Combine(basePath, "private.pem");

            File.WriteAllText(publicKeyPath, publicKey);
            File.WriteAllText(privateKeyPath, privateKey);
        }
    }
}
