using EncryptedChatApp.Avalonia.Api;
using EncryptedChatApp.Avalonia.Core.Interfaces;
using EncryptedChatApp.Avalonia.Models;
using ReactiveUI;
using System.Reactive;
using System.Threading.Tasks;

namespace EncryptedChatApp.Avalonia.ViewModels
{
    public class RegisterPageViewModel : ViewModelBase
    {
        private string userName;
        public string UserName
        {
            get => userName;
            set => this.RaiseAndSetIfChanged(ref userName, value);
        }

        private string email;
        public string Email
        {
            get => email;
            set => this.RaiseAndSetIfChanged(ref email, value);
        }

        private string password;
        public string Password
        {
            get => password;
            set => this.RaiseAndSetIfChanged(ref password, value);
        }

        private string confirmPassword;
        public string ConfirmPassword
        {
            get => confirmPassword;
            set => this.RaiseAndSetIfChanged(ref confirmPassword, value);
        }

        private bool completed;
        public bool Completed
        {
            get => completed;
            set => this.RaiseAndSetIfChanged(ref completed, value);
        }


        public CurrentUserManager CurrentUserManager => userManager;

        private readonly CurrentUserManager userManager;
        private readonly ApiClient client;
        private readonly IKeyGenerator keyGenerator;
        private readonly IKeyStore keyStore;

        public RegisterPageViewModel(CurrentUserManager userManager, ApiClient client, IKeyGenerator keyGenerator, IKeyStore keyStore)
        {
            this.userManager = userManager;
            this.client = client;
            this.keyGenerator = keyGenerator;
            this.keyStore = keyStore;
            RegisterCommand = ReactiveCommand.CreateFromTask(Register);
        }

        public ReactiveCommand<Unit, Unit> RegisterCommand { get; }

        async Task Register()
        {
            keyGenerator.CreateKeyPair(userName, persist: true);

            var publicKey = keyStore.GetPublicKeyString(userName);

            var model = new RegisterModel()
            {
                Email = email,
                PublicKey = publicKey,
                UserName = userName,
                Password = password,
                ConfirmPassword = password
            };

            var result = await client.CreateAccount(model);

            if (!result.Success)
            {
                var messageBoxErrorWindow = MessageBox.Avalonia.MessageBoxManager
                        .GetMessageBoxStandardWindow("Registration failed", result.ErrorMessage);

                await messageBoxErrorWindow.Show();

                return;
            }

            Completed = true;
        }
    }
}
