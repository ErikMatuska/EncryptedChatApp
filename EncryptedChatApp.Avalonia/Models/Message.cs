namespace EncryptedChatApp.Avalonia.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int RecipientId { get; set; }

        public string Key { get; set; }
        public string Content { get; set; }
    }
}
