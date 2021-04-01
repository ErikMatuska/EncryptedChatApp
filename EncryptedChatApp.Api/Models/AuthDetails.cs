using Microsoft.AspNetCore.Mvc;
using System;

namespace EncryptedChatApp.Api.Controllers
{
    public class AuthDetails
    {
        public string Token { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public DateTime ExpireDate { get; set; }
        public int UserId { get; set; }
    }
}
