using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using EncryptedChatApp.Avalonia.ViewModels;
using Splat;

namespace EncryptedChatApp.Avalonia.Pages
{
    public interface ILoadOnAppearingPage
    {
        public void OnAppearing();
    }

    public class LoginPage : UserControl, ILoadOnAppearingPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        public void OnAppearing()
        {
            DataContext = Locator.Current.GetService<LoginPageViewModel>();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
