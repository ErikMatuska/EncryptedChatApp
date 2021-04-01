using CommandLine;
using EncryptedChatApp.Console.Models;
using Microsoft.Diagnostics.Runtime.ICorDebug;
using Newtonsoft.Json;
using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection.Metadata;
using System.Resources;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EncryptedChatApp.Console
{
    public class Program
    {
        //private const string Url = "https://localhost:5001/api";
        private const string Url = "https://encryption.ermo.cloud/api";

        public static string BasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ChatEncryptedApp");
        public static string Config = Path.Combine(BasePath, "config.data");


        private static async Task Main(string[] args)
        {
            if (!Directory.Exists(Program.BasePath))
            {
                Directory.CreateDirectory(Program.BasePath);
            }

            await CommandLine.Parser.Default.ParseArguments<
                   RegisterOptions,
                    LoginOptions,
                    ListUsersOptions,
                    GetMessagesOptions,
                    SendOptions>(args).MapResult(
                         (RegisterOptions _) => Register(),
                         (LoginOptions _) => Login(),
                         (GetMessagesOptions opts) => GetMessages(opts),
                         (ListUsersOptions opts) => DisplayUsers(),
                         (SendOptions opts) => Send(opts),
                         errs => Task.FromResult(0));
        }

        private static async Task Login()
        {
            System.Console.WriteLine("Please enter your account details: ");

            System.Console.Write("Enter username: ");
            var username = System.Console.ReadLine();
            System.Console.Write("Enter password: ");
            var password = System.Console.ReadLine();

            var httpClient = new HttpClient();

            var model = new LoginModel()
            {
                UserName = username,
                Password = password,
            };

            var postRequest = new HttpRequestMessage(HttpMethod.Post, new Uri($"{Url}/auth/login"))
            {
                Content = JsonContent.Create(model)
            };

            var response = await httpClient.SendAsync(postRequest);

            if (!response.IsSuccessStatusCode)
            {
                System.Console.WriteLine(await response.Content.ReadAsStringAsync());
                return;
            }

            var content = await response.Content.ReadAsStringAsync();
            var tokenDetails = JsonConvert.DeserializeObject<AuthDetails>(content);

            var currentUserInfo = new CurrentUserInfo()
            {
                Token = tokenDetails.Token,
                TokenExpiresIn = tokenDetails.ExpireDate,
                UserId = tokenDetails.UserId,
                Username = username
            };

            System.Console.WriteLine("Logged in! Load current users - use 'list'");

            File.WriteAllText(Config, JsonConvert.SerializeObject(currentUserInfo));

            await LoadContacts();
        }

        private static async Task LoadContacts()
        {
            var currentUser = ContactsExtensions.GetCurrentUser();

            var path = Path.Combine(BasePath, $"{currentUser.Username}.data/contacts.data");

            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", currentUser.Token);

            var users = await httpClient.GetFromJsonAsync<List<UserDetail>>($"{Url}/messaging/list");

            File.WriteAllText(Path.Combine(BasePath, $"{currentUser.Username}.data/contacts.data"), JsonConvert.SerializeObject(users));
        }

        private static string GetUserPublicKeyByName(string userName)
        {
            var currentUser = ContactsExtensions.GetCurrentUser();

            var users = JsonConvert.DeserializeObject<List<UserDetail>>(
                File.ReadAllText(Path.Combine(BasePath, $"{currentUser.Username}.data/contacts.data")));

            var user = users.FirstOrDefault(u => u.Username == userName);

            return user.PublicKey;
        }

        private static async Task Register()
        {
            System.Console.WriteLine("Please enter your new account details: ");

            System.Console.Write("Enter email: ");
            var email = System.Console.ReadLine();
            System.Console.Write("Enter username: ");
            var username = System.Console.ReadLine();
            System.Console.Write("Enter password: ");
            var password = System.Console.ReadLine();

            var httpClient = new HttpClient();
            var keyManager = new KeyManager();

            keyManager.CreateKeys(username);

            var publicKey = keyManager.GetPublicKeyFromFileString(Path.Combine(BasePath, $"{username}.data/{username}.public.pem"));

            var model = new RegisterModel()
            {
                Email = email,
                PublicKey = publicKey,
                UserName = username,
                Password = password,
                ConfirmPassword = password
            };

            var postRequest = new HttpRequestMessage(HttpMethod.Post, new Uri($"{Url}/auth/register"))
            {
                Content = JsonContent.Create(model)
            };

            var response = await httpClient.SendAsync(postRequest);

            System.Console.WriteLine(response.StatusCode);

            System.Console.WriteLine(response.IsSuccessStatusCode ? "Register succesfull, please log in!" : await response.Content.ReadAsStringAsync());
        }

        private static async Task GetMessages(GetMessagesOptions opts)
        {
            var currentUser = ContactsExtensions.GetCurrentUser();

            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", currentUser.Token);

            var messages = await httpClient.GetFromJsonAsync<List<Message>>($"{Url}/messaging/messages");

            if (messages != null)
            {
                var keyManager = new KeyManager();
                var privateKey = keyManager.ReadPrivateKeyFromFile(Path.Combine(BasePath, $"{currentUser.Username}.data/{currentUser.Username}.private.pem"));

                foreach (var message in messages)
                {
                    var key = keyManager.RsaDecrypt(message.Key, privateKey);
                    var messagePlainText = keyManager.AesDecrypt(message.Content, key);

                    System.Console.WriteLine($"{ContactsExtensions.GetUserNameById(message.SenderId)}: {messagePlainText}");

                    if (opts.Debug)
                    {
                        System.Console.WriteLine("Received:");
                        System.Console.WriteLine(JsonConvert.SerializeObject(messages));
                    }
                }
            }
            else
            {
                throw new Exception("messaging/messages failed");
            }
        }

        private static async Task DisplayUsers()
        {
            var currentUser = ContactsExtensions.GetCurrentUser();

            await LoadContacts();

            var users = JsonConvert.DeserializeObject<List<UserDetail>>(File.ReadAllText(Path.Combine(BasePath, $"{currentUser.Username}.data/contacts.data")));

            System.Console.WriteLine("Users: ");
            foreach (var user in users)
            {
                System.Console.WriteLine(user.Username);
            }
        }

        private static async Task Send(SendOptions opts)
        {
            var currentUser = ContactsExtensions.GetCurrentUser();

            if (opts.To == currentUser.Username)
            {
                System.Console.WriteLine("Cannot send message to yourself");
                return;
            }

            var keyManager = new KeyManager();

            var recipientPublicKeyString = GetUserPublicKeyByName(opts.To);

            var publicKey = keyManager.LoadPublicKey(recipientPublicKeyString);

            var (encryptedMessage, aesKey) = keyManager.AesEncrypt(opts.Message);

            var encryptedKey = keyManager.RsaEncrypt(aesKey, publicKey);

            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", currentUser.Token);

            var message = new Message()
            {
                Key = encryptedKey,
                Content = encryptedMessage,
                SenderId = currentUser.UserId,
                RecipientId = ContactsExtensions.GetUserIdByName(opts.To)
            };

            if (opts.Debug)
            {
                System.Console.WriteLine("Data sent:");
                System.Console.WriteLine(JsonConvert.SerializeObject(message));
            }

            var postRequest = new HttpRequestMessage(HttpMethod.Post, new Uri($"{Url}/messaging/send"))
            {
                Content = JsonContent.Create(message)
            };

            var response = await httpClient.SendAsync(postRequest);

            response.EnsureSuccessStatusCode();
        }
    }
}
