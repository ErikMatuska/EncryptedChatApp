using CommandLine;

namespace EncryptedChatApp.Console
{
    [Verb("create",
        HelpText = "INITIALIZE COMMAND, creates your private and public key (public key is sent to the server)")]
    public class CreateOptions : Options
    {

    }
}
