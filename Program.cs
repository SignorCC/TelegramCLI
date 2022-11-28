using CommandLine;
using System.Collections;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using TL;

namespace Telegram_CLI
{
    internal class Program
    {
        // Constants
        private static string BOTTOKEN;
        private static string BOTPASSWORD;
        private static int API_ID;
        private static string API_HASH;
        private static string PHONE;

        static async Task Main(string[] args)
        {
            // Redirect WTelegram Output Stream
            WTelegram.Helpers.Log = (lvl, str) => log(lvl, str);

            if(ReadEnvironmentVariables());

            else
                if (System.IO.File.Exists("config.txt"))
                    ReadVariablesFromConfig();
            

            string result = "void";

            result =  await Parser.Default.ParseArguments<SendMessageCommand,CreateSessionCommand>(args)
                .MapResult<SendMessageCommand,CreateSessionCommand, Task<string>>
                (
                    (SendMessageCommand opts) => SendMessage(opts),
                    (CreateSessionCommand opts) => CreateSession(opts),
                    errs => Error()
                );

            if(result != "void")
                Console.WriteLine(result);

        }

        private static async Task<string> SendMessage(SendMessageCommand opts)
        {
            try
            {
                WrapperClient wrapperClient = await WrapperClient.Create(BOTTOKEN, API_ID, API_HASH, PHONE, BOTPASSWORD);

                if (opts.Phone == null && opts.User != null)
                    return await wrapperClient.SendMessageToUser(opts.User, opts.Message);

                else
                  if(opts.Phone != null)
                    return await wrapperClient.SendMessageToUser(opts.Phone, opts.Message, true);
            }

            catch(Exception e)
            {
                return $"Error creating Client: {e.Message}";
            }

            return $"Error sending Message - Did you provide user/phone?";

        }

        private static async Task<string> CreateSession(CreateSessionCommand opts)
        {
            WrapperClient wrapperClient = await WrapperClient.Create(BOTTOKEN, API_ID, API_HASH, PHONE, BOTPASSWORD);

            return "Client created successfully!";
        }

        
        private static async Task<string> Error()
        {
            return "Generic Error";
        }

        private static void log(int lvl, String str)
        {
            // TO DO: implement logging
        }

        private static bool ReadEnvironmentVariables()
        {
            try
            {
                BOTTOKEN = Environment.GetEnvironmentVariable("BOTTOKEN", EnvironmentVariableTarget.Machine);
                BOTPASSWORD = Environment.GetEnvironmentVariable("BOTPASSWORD", EnvironmentVariableTarget.Machine);
                int.TryParse(Environment.GetEnvironmentVariable("API_ID", EnvironmentVariableTarget.Machine), out API_ID);
                API_HASH = Environment.GetEnvironmentVariable("API_HASH", EnvironmentVariableTarget.Machine);
                PHONE = Environment.GetEnvironmentVariable("PHONE", EnvironmentVariableTarget.Machine);
                return true;
            }

            catch
            {
                return false;
            }
            
        }

        private static bool ReadVariablesFromConfig()
        {
            try
            {
                List<string> ret = new List<string>();

                string[] read = System.IO.File.ReadAllLines("config.txt");

                foreach (string s in read)
                    ret.Add(s);

                string[] cut;

                cut = ret[1].Split("=");
                BOTTOKEN = cut[1];

                cut = ret[2].Split("=");
                BOTPASSWORD = cut[1];

                cut = ret[3].Split("=");
                int.TryParse(cut[1], out API_ID);

                cut = ret[4].Split("=");
                API_HASH = cut[1];

                cut = ret[5].Split("=");
                PHONE = cut[1];

                return true;
            }

            catch
            {
                return false;
            }

        }


    }
}