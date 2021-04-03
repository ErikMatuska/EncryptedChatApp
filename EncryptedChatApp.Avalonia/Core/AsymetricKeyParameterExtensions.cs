using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using System.IO;
using System.Security.Cryptography;

namespace EncryptedChatApp.Avalonia.Core
{
    public class AsymetricKeyParameterExtensions
    {
        public static string GetKeyStringFromParameter(AsymmetricKeyParameter key)
        {
            var textWriter = new StringWriter();
            var pemWriter = new PemWriter(textWriter);

            pemWriter.WriteObject(key);
            pemWriter.Writer.Flush();

            return textWriter.ToString();
        }

        public static RSAParameters GetPublicKeyFromString(string content)
        {
            var rsa = RSA.Create();

            rsa.ImportFromPem(content.ToCharArray());

            return rsa.ExportParameters(false);
        }
    }
}
