using System;

namespace EncryptedChatApp.Avalonia.Models
{
    public class CurrentUserInfo
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public int UserId { get; set; }
        public DateTime TokenExpiresIn { get; set; }
    }
}
