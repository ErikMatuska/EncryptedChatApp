using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using EncryptedChatApp.Avalonia.ViewModels;
using Splat;

namespace EncryptedChatApp.Avalonia.Pages
{
    public class MessagesPage : UserControl, ILoadOnAppearingPage
    {
        public MessagesPage()
        {
            InitializeComponent();
        }

        public void OnAppearing()
        {
            DataContext = Locator.Current.GetService<MessagesPageViewModel>();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
