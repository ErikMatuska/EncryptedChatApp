using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Threading.Tasks;
using EncryptedChatApp.Api.Data;
using EncryptedChatApp.Api.Models;

namespace EncryptedChatApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessagingController : ControllerBase
    {
        private readonly ILogger<MessagingController> logger;
        private readonly ApplicationDbContext context;

        public MessagingController(ILogger<MessagingController> logger, ApplicationDbContext context)
        {
            this.logger = logger;
            this.context = context;
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("hoj");
        }

        [HttpPost("send")]
        public async Task<IActionResult> Send(Message message)
        {
            logger.LogInformation(message.Content);

            var entity = await context.Messages.AddAsync(message);

            await context.SaveChangesAsync();

            logger.LogInformation($"Message sent from {message.SenderId} to {message.RecipientId}");

            return Ok();
        }

        [HttpGet("messages")]
        public async Task<IActionResult> GetMessages()
        {
            var userid = UserExtensions.GetUserId(User);

            var messages = await context.Messages.Where(m => m.RecipientId == userid).ToListAsync();

            if (messages.Count>0)
            {
                Console.WriteLine("yeas");
            }

            return Ok(messages);
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await context.Users.Select(u => new UserDetail()
            {
                PublicKey = u.Key,
                UserId = u.Id,
                Username = u.UserName
            }).ToListAsync();

            return Ok(users);
        }
    }

    public class CreateRequestModel
    {
        public string Name { get; set; }
        public string PublicKey { get; set; }
    }

    public class UserDetail
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string PublicKey { get; set; }
    }

    //public class MessageStore
    //{
    //    private const string Path = "store/messages.data";

    //    public static void AddMessage(SendMessageRequestModel model)
    //    {
    //        List<Message> messages = new List<Message>();

    //        if (File.Exists(Path))
    //        {
    //            messages = JsonConvert.DeserializeObject<List<SendMessageRequestModel>>(File.ReadAllText(Path));
    //        }

    //        messages.Add(model);

    //        File.WriteAllText(Path, JsonConvert.SerializeObject(messages));
    //    }

    //    public static List<SendMessageRequestModel> GetMessagesFor(string recipient)
    //    {
    //        var messages = JsonConvert.DeserializeObject<List<SendMessageRequestModel>>(File.ReadAllText(Path));

    //        return messages.Where(m => m.Recipient == recipient).ToList();
    //    }
    //}

    //public class DataStore
    //{
    //    private const string Path = "store/store.data";

    //    public static void AddRecord(CreateRequestModel model)
    //    {
    //        Dictionary<string, string> data = new Dictionary<string, string>();

    //        if (File.Exists(Path))
    //        {
    //            data = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(Path));
    //        }

    //        data[model.Name] = model.PublicKey;

    //        File.WriteAllText(Path, JsonConvert.SerializeObject(data));
    //    }

    //    public static Dictionary<string, string> GetUsers()
    //    {
    //        return JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(Path));
    //    }
    //}
}
