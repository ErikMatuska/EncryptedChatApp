using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using EncryptedChatApp.Avalonia.ViewModels;
using EncryptedChatApp.Avalonia.Views;
using Splat;

namespace EncryptedChatApp.Avalonia
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            Bootstrapper.Register(Locator.CurrentMutable, Locator.Current); 

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    //DataContext = new MainWindowViewModel(),
                    DataContext = Locator.Current.GetService<MainWindowViewModel>()
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
