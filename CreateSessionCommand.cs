using CommandLine.Text;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telegram_CLI
{
    [Verb("CreateSession", HelpText = "Create a new Telegram Session")]
    public class CreateSessionCommand : ICommand
    {

        [Option('p', "phone", Required = false, HelpText = "Phone number in international format")]
        public String? Phone { get; set; }
        public void Execute()
        {
            Console.WriteLine("Creating Session");
        }
    }
}
