namespace EncryptedChatApp.Avalonia.Core.Interfaces
{
    public interface IPublicKeyProvider
    {
        public string GetPublicKeyString(string folderName);
    }

}
