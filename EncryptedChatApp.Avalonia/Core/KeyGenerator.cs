using EncryptedChatApp.Avalonia.Core.Interfaces;
using Org.BouncyCastle.Security;
using System.Security.Cryptography;

namespace EncryptedChatApp.Avalonia.Core
{
    public class KeyGenerator : IKeyGenerator
    {
        private readonly IKeyStore keyStore;

        public KeyGenerator(IKeyStore keyStore)
        {
            this.keyStore = keyStore;
        }

        public void CreateKeyPair(string folderName, bool persist = true)
        {
            var csp = new RSACryptoServiceProvider(4096);

            var rsaKeyPair = DotNetUtilities.GetRsaKeyPair(csp.ExportParameters(true));

            if (persist)
                keyStore.SaveKeyPair(rsaKeyPair, folderName);
        }
    }
}
