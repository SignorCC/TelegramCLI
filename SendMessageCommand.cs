using CommandLine.Text;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telegram_CLI
{
    [Verb("SendMessage", HelpText = "Send a Message to a user based on username or phone number")]
    public class SendMessageCommand : ICommand
    {
        [Option('m', "message", Required = true, HelpText = "String to be sent")]
        public string Message { get; set; }

        [Option('u', "user", Required = false, HelpText = "Username")]
        public String? User { get; set; }

        [Option('p', "phone", Required = false, HelpText = "Phone number in international format")]
        public String? Phone { get; set; }
        public void Execute()
        {
            Console.WriteLine("Sending Message");
        }
    }
}
