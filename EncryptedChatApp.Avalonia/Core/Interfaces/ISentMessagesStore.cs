using EncryptedChatApp.Avalonia.Fonts;
using EncryptedChatApp.Avalonia.Models;
using System.Collections.Generic;
using System.Linq;

namespace EncryptedChatApp.Avalonia.Core.Interfaces
{
    public interface ISentMessagesStore
    {
        public void SaveMessage(Message message);
        public IEnumerable<Message> GetMessages();
    }

    public class MessagesStore : ISentMessagesStore
    {
        public IEnumerable<Message> GetMessages()
        {
            return new MessagingDbContext().Messages.ToList();
        }

        public void SaveMessage(Message message)
        {
            using (var db = new MessagingDbContext())
            {
                db.Messages.Add(message);
                db.SaveChanges();
            }
        }
    }
}
