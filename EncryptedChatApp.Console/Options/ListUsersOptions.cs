using CommandLine;

namespace EncryptedChatApp.Console
{
    [Verb("list", HelpText = "View all users and load their PUBLIC KEYS")]
    public class ListUsersOptions : Options
    {

    }
}
