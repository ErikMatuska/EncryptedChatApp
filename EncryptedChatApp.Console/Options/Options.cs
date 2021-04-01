using CommandLine;

namespace EncryptedChatApp.Console
{
    public class Options
    {
        //[Value(0, Required = true, HelpText = "YOUR USERNAME!")]
        //public string From { get; set; }

        [Option('d', "debuh", Default = false, HelpText = "Show DEBUG data")]
        public bool Debug { get; set; }
    }
}
