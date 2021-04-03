using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using EncryptedChatApp.Avalonia.ViewModels;
using Splat;

namespace EncryptedChatApp.Avalonia.Pages
{
    public class UserInfoPage : UserControl, ILoadOnAppearingPage
    {
        public UserInfoPage()
        {
            InitializeComponent();
        }

        public void OnAppearing()
        {
            DataContext = Locator.Current.GetService<UserInfoPageViewModel>();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
