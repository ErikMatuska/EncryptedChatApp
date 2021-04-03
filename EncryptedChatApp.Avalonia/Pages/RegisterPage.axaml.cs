using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using EncryptedChatApp.Avalonia.ViewModels;
using Splat;

namespace EncryptedChatApp.Avalonia.Pages
{
    public class RegisterPage : UserControl
    {
        public RegisterPage()
        {
            InitializeComponent();

            DataContext = Locator.Current.GetService<RegisterPageViewModel>();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
