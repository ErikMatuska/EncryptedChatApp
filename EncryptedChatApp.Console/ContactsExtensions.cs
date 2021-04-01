using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EncryptedChatApp.Console
{
    public class ContactsExtensions
    {
        public static CurrentUserInfo GetCurrentUser()
        {
            if (!File.Exists(Program.Config))
                return null;

            var content = File.ReadAllText(Program.Config);
            var info = JsonConvert.DeserializeObject<CurrentUserInfo>(content);

            return info;
        }

        public static int GetUserIdByName(string userName)
        {
            var currentUser = GetCurrentUser();

            var users = JsonConvert.DeserializeObject<List<UserDetail>>(File.ReadAllText(Path.Combine(Program.BasePath, $"{currentUser.Username}.data/contacts.data")));

            var user = users.FirstOrDefault(u => u.Username == userName);

            return user.UserId;
        }

        public static string GetUserNameById(int userId)
        {
            var currentUser = GetCurrentUser();

            var users = JsonConvert.DeserializeObject<List<UserDetail>>(File.ReadAllText(Path.Combine(Program.BasePath, $"{currentUser.Username}.data/contacts.data")));

            var user = users.FirstOrDefault(u => u.UserId == userId);

            return user.Username;
        }
    }
}
