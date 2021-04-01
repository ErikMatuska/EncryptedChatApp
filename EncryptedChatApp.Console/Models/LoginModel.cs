using System.ComponentModel.DataAnnotations;

namespace EncryptedChatApp.Console.Models
{
    public class LoginModel
    {
        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string Password { get; set; }
    }
}
