using Avalonia;
using Avalonia.Markup.Xaml;
using EncryptedChatApp.Avalonia.Api;
using EncryptedChatApp.Avalonia.Core;
using EncryptedChatApp.Avalonia.Core.Interfaces;
using EncryptedChatApp.Avalonia.ViewModels;
using Splat;
using System.Net.Http;
using System.Net.Http.Headers;

namespace EncryptedChatApp.Avalonia
{
    public static class Bootstrapper
    {
        public static void Register(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.RegisterLazySingleton(() => new CurrentUserManager());

            services.Register(() => new MainWindowViewModel(
                resolver.GetService<CurrentUserManager>()
            ));

            services.Register(() => new LoginPageViewModel(
                resolver.GetService<CurrentUserManager>(),
                resolver.GetService<ApiClient>()
            ));

            services.Register(() => new UserInfoPageViewModel(
                resolver.GetService<CurrentUserManager>(),
                                resolver.GetService<IKeyStore>()
            ));

            services.Register(() => new RegisterPageViewModel(
                resolver.GetService<CurrentUserManager>(),
                resolver.GetService<ApiClient>(),
                resolver.GetService<IKeyGenerator>(),
                resolver.GetService<IKeyStore>()
            ));

            services.Register(() => new MessagesPageViewModel(
                resolver.GetService<ApiClient>(),
                resolver.GetService<CurrentUserManager>(),
                resolver.GetService<IAsymetricEncryptionService>(),
                resolver.GetService<ISymetricEnryptionService>(),
                resolver.GetService<IKeyStore>()
            ));

            services.Register(() => {
                var client = new HttpClient();
                var token = resolver.GetService<CurrentUserManager>().Token;

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                return client;
            });

            services.Register(() => new ApiClient(resolver.GetService<HttpClient>()));


            services.Register<IKeyGenerator>(() => new KeyGenerator(resolver.GetService<IKeyStore>()));

            services.Register<IKeyStore>(() => new AppDataKeyStore());

            services.Register<IAsymetricEncryptionService>(() =>
                new AsymetricEnryptionService(
                    resolver.GetService<IKeyStore>(),
                    resolver.GetService<CurrentUserManager>()));

            services.Register<ISymetricEnryptionService>(() => new AesSymetricEnryptionService());

            services.Register<ISentMessagesStore>(() => null);
        }
    }
}
