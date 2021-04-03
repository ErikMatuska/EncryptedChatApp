using Avalonia.Controls;
using Avalonia.Dialogs;
using EncryptedChatApp.Avalonia.Api;
using EncryptedChatApp.Avalonia.Models;
using ReactiveUI;
using System;
using System.Reactive;
using System.Threading.Tasks;

namespace EncryptedChatApp.Avalonia.ViewModels
{

    public class LoginPageViewModel : ViewModelBase
    {
        private string userName;
        public string UserName
        {
            get => userName;
            set => this.RaiseAndSetIfChanged(ref userName, value);
        }

        private string password;
        public string Password
        {
            get => password;
            set => this.RaiseAndSetIfChanged(ref password, value);
        }

        public CurrentUserManager CurrentUserManager => userManager;

        private readonly CurrentUserManager userManager;
        private readonly ApiClient client;

        public LoginPageViewModel(CurrentUserManager userManager, ApiClient client)
        {
            this.userManager = userManager;
            this.client = client;

            LoginCommand = ReactiveCommand.CreateFromTask(Login);
        }

        public ReactiveCommand<Unit, Unit> LoginCommand { get; }

        async Task Login()
        {
            var model = new LoginModel()
            {
                UserName = userName,
                Password = password,
            };

            var result = await client.Authenticate(model);

            if (!result.Success)
            {
                var messageBoxErrorWindow = MessageBox.Avalonia.MessageBoxManager
                        .GetMessageBoxStandardWindow("Login failed", result.ErrorMessage);

                await messageBoxErrorWindow.Show();

                return;
            }

            userManager.IsLogged = true;
            userManager.UserName = userName;
            userManager.Token = result.Data.Token;
            userManager.UserId = result.Data.UserId;
        }
    }
}
