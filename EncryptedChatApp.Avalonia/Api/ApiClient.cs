using EncryptedChatApp.Avalonia.Core.Interfaces;
using EncryptedChatApp.Avalonia.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace EncryptedChatApp.Avalonia.Api
{
    public class ApiClient
    {
        private const string Url = "https://encryption.ermo.cloud/api";

        private readonly HttpClient httpClient;

        public ApiClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<List<Message>> GetMessages()
        {
            return await httpClient.GetFromJsonAsync<List<Message>>($"{Url}/messaging/messages");
        }

        public async Task<Result<CurrentUserInfo>> Authenticate(LoginModel model)
        {
            var postRequest = new HttpRequestMessage(HttpMethod.Post, new Uri($"{Url}/auth/login"))
            {
                Content = JsonContent.Create(model)
            };

            HttpResponseMessage? response;

            try
            {
                response = await httpClient.SendAsync(postRequest);
            }
            catch (Exception e)
            {
                return new Result<CurrentUserInfo>(e.ToString());
            }

            if (!response.IsSuccessStatusCode)
            {
                return new Result<CurrentUserInfo>(await response.Content.ReadAsStringAsync());
            }

            var content = await response.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<AuthDetails>(content);

            var currentUserInfo = new CurrentUserInfo()
            {
                Token = token.Token,
                TokenExpiresIn = token.ExpireDate,
                UserId = token.UserId,
                Username = model.UserName
            };

            return new Result<CurrentUserInfo>(currentUserInfo);
        }

        public async Task<Result<object>> CreateAccount(RegisterModel model)
        {
            var postRequest = new HttpRequestMessage(HttpMethod.Post, new Uri($"{Url}/auth/register"))
            {
                Content = JsonContent.Create(model)
            };

            HttpResponseMessage? response;

            try
            {
                response = await httpClient.SendAsync(postRequest);
            }
            catch (Exception e)
            {
                return new Result<object>(e.ToString());
            }

            if (!response.IsSuccessStatusCode)
            {
                return new Result<object>(await response.Content.ReadAsStringAsync());
            }

            return new Result<object>(true);
        }

        public async Task SendMessage(Message message)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, new Uri($"{Url}/messaging/send"))
            {
                Content = JsonContent.Create(message)
            };

            var response = await httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();
        }
    }

    public class Result<T>
    {
        public string ErrorMessage { get; }
        public bool Success { get; }
        public T Data { get; }

        public Result()
        {
            Success = false;
        }

        public Result(bool success)
        {
            Success = success;
        }

        public Result(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public Result(T data)
        {
            Data = data;
            Success = true;
        }
    }
}
