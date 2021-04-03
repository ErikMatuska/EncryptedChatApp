using EncryptedChatApp.Avalonia.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ReactiveUI;
using System.Reactive;

namespace EncryptedChatApp.Avalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public CurrentUserManager CurrentUserManager => currentUserManager;

        private CurrentUserManager currentUserManager;

        public MainWindowViewModel(CurrentUserManager currentUserManager)
        {
            this.currentUserManager = currentUserManager;

            LogoutCommand = ReactiveCommand.CreateFromTask(currentUserManager.Logout);
        }

        public ReactiveCommand<Unit, Unit> LogoutCommand { get; }

        private string text = "hoj";
        public string Text
        {
            get => text;
            set => this.RaiseAndSetIfChanged(ref text, value);
        }

        public void Load()
        {

            var keyManager = new KeyManager();

            var username = "roman";

            keyManager.CreateKeys(username);

            var publicKey = keyManager.GetPublicKeyFromFileString(Path.Combine(Program.BasePath, $"{username}.data/{username}.public.pem"));

            Text = publicKey;
        }
    }
}
