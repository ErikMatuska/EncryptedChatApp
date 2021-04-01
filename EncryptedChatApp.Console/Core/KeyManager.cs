using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace EncryptedChatApp.Console
{
    public class KeyManager
    {
        public static bool KeysExist(string name)
        {
            return File.Exists(Path.Combine(Program.BasePath, $"{name}.data/{name}.private.pem")) && File.Exists(Path.Combine(Program.BasePath, $"{name}.data/{name}.public.pem"));
        }

        public RSAParameters ReadPrivateKeyFromFile(string privateKeyFileName) => ReadPrivateKey(File.ReadAllText(privateKeyFileName));
        public RSAParameters ReadPrivateKey(string pemContent)
        {
            var rsa = RSA.Create();
            rsa.ImportFromPem(pemContent.ToCharArray());

            return rsa.ExportParameters(true);
        }

        public string GetPublicKeyFromFileString(string publicKeyFileName) => File.ReadAllText(publicKeyFileName);
        public RSAParameters LoadPublicKeyFromFile(string publicKeyFileName) => LoadPublicKey(File.ReadAllText(publicKeyFileName));
        public RSAParameters LoadPublicKey(string pemContent)
        {
            var rsa = RSA.Create();
            rsa.ImportFromPem(pemContent.ToCharArray());

            return rsa.ExportParameters(false);
        }

        public void CreateKeys(string name)
        {
            var csp = new RSACryptoServiceProvider(4096);

            var keyPair = DotNetUtilities.GetRsaKeyPair(csp.ExportParameters(true));

            Directory.CreateDirectory(Path.Combine(Program.BasePath, $"{name}.data"));

            File.WriteAllText(Path.Combine(Program.BasePath, $"{name}.data/{name}.public.pem"), GetKeyString(keyPair.Public));
            File.WriteAllText(Path.Combine(Program.BasePath, $"{name}.data/{name}.private.pem"), GetKeyString(keyPair.Private));
        }

        public static string GetKeyString(AsymmetricKeyParameter key)
        {
            var textWriter = new StringWriter();
            var pemWriter = new PemWriter(textWriter);

            pemWriter.WriteObject(key);
            pemWriter.Writer.Flush();

            return textWriter.ToString();
        }

        public (string cypherText, string key) AesEncrypt(string plainText)
        {
            var rnd = new RNGCryptoServiceProvider();
            var key = new byte[16];

            rnd.GetNonZeroBytes(key);

            var aes = new AesBcCrypto(CipherMode.GCM, Padding.NoPadding);

            var encrypted = aes.Encrypt(plainText, key);

            return (encrypted, Convert.ToBase64String(key));
        }

        public string AesDecrypt(string cypherText, string key)
        {
            var keyBytes = Convert.FromBase64String(key);

            var aesLib = new AesBcCrypto(CipherMode.GCM, Padding.NoPadding);

            return aesLib.Decrypt(cypherText, keyBytes);
        }

        public string RsaEncrypt(string text, RSAParameters publicKey)
        {
            var csp = new RSACryptoServiceProvider(2048);
            csp.ImportParameters(publicKey);

            var bytesPlainText = Encoding.Unicode.GetBytes(text);
            var bytesCypherText = csp.Encrypt(bytesPlainText, false);
            return Convert.ToBase64String(bytesCypherText);
        }

        public string RsaDecrypt(string text, RSAParameters privateKey)
        {
            var csp = new RSACryptoServiceProvider(2048);
            csp.ImportParameters(privateKey);

            var bytesCypherText = Convert.FromBase64String(text);
            var bytesPlainText = csp.Decrypt(bytesCypherText, false);
            return Encoding.Unicode.GetString(bytesPlainText);
        }
    }
}
