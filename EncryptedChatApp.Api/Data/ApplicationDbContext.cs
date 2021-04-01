using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EncryptedChatApp.Api.Data
{
    public class ApplicationDbContext : IdentityDbContext<ChatUser, ChatRole, int>
    {
        public DbSet<Message> Messages { get; set; }

        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            base.OnModelCreating(builder);
        }
    }

    public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int RecipientId { get; set; }

        public string Key { get; set; }
        public string Content { get; set; }

        public ChatUser Sender { get; set; }
        public ChatUser Recipient { get; set; }
    }

    public class ChatUser : IdentityUser<int>
    {
        public string Key { get; set; }
    }

    public class ChatRole : IdentityRole<int>
    {

    }
}
