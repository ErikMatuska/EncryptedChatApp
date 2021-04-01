using CommandLine;

namespace EncryptedChatApp.Console
{
    [Verb("send", HelpText = "Send message to recipient, specify RECiPIENT and MESSAGE")]
    public class SendOptions : Options
    {
        [Option('t', "to", HelpText = "Username of the Recipient", Required = true)]
        public string To { get; set; }

        [Option('m', "message", HelpText = "Message", Required = true)]
        public string Message { get; set; }
    }
}
