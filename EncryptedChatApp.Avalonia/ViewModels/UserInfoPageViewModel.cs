using EncryptedChatApp.Avalonia.Core.Interfaces;
using ReactiveUI;

namespace EncryptedChatApp.Avalonia.ViewModels
{

    public class UserInfoPageViewModel : ViewModelBase
    {
        private string publicKey;
        public string PublicKey
        {
            get => publicKey;
            set => this.RaiseAndSetIfChanged(ref publicKey, value);
        }

        private string privateKey;
        public string PrivateKey
        {
            get => publicKey;
            set => this.RaiseAndSetIfChanged(ref privateKey, value);
        }

        public CurrentUserManager CurrentUserManager => currentUserManager;


        private readonly CurrentUserManager currentUserManager;
        private readonly IKeyStore keyStore;

        public UserInfoPageViewModel(CurrentUserManager currentUserManager, IKeyStore keyStore)
        {
            this.currentUserManager = currentUserManager;
            this.keyStore = keyStore;

            if (currentUserManager.IsLogged)
            {
                PublicKey = keyStore.GetPublicKeyString(currentUserManager.GetFolderName());
                PrivateKey = keyStore.GetPrivateKeyString(currentUserManager.GetFolderName());
            }
        }
    }
}
