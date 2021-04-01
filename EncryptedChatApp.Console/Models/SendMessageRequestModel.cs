namespace EncryptedChatApp.Console
{
    public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int RecipientId { get; set; }

        public string Key { get; set; }
        public string Content { get; set; }

        //public ChatUser SenderId { get; set; }
        //public ChatUser Recipient { get; set; }
    }

}
