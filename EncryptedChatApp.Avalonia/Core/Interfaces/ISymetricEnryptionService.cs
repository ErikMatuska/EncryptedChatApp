namespace EncryptedChatApp.Avalonia.Core.Interfaces
{
    public interface ISymetricEnryptionService
    {
        public SymetricEnryptionResult Enrypt(string plainTextMessage);
        public string Decrypt(string cipherTextMessage, string key);
    }
}
