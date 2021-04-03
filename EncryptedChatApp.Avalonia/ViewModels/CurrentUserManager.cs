using ReactiveUI;
using System.Threading.Tasks;

namespace EncryptedChatApp.Avalonia.ViewModels
{
    public class CurrentUserManager : ReactiveObject, IFolderProvider
    {
        private bool isLogged;
        public bool IsLogged
        {
            get => isLogged;
            set => this.RaiseAndSetIfChanged(ref isLogged, value);
        }

        private string userName;
        public string UserName
        {
            get => userName;
            set => this.RaiseAndSetIfChanged(ref userName, value);
        }

        private string token;
        public string Token
        {
            get => token;
            set => this.RaiseAndSetIfChanged(ref token, value);
        }

        private int userId;
        public int UserId
        {
            get => userId;
            set => this.RaiseAndSetIfChanged(ref userId, value);
        }

        public async Task Logout()
        {
            userName = null;

            IsLogged = false;
        }

        public string GetFolderName() => UserName;
    }

    public interface IFolderProvider
    {
        public string GetFolderName();
    }
}
