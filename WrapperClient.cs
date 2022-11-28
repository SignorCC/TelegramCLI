using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TL;
using WTelegram;

namespace Telegram_CLI
{
    public class WrapperClient
    {
        public WTelegram.Client client;
        private string botToken;
        private int API_ID;
        private string API_HASH;
        private string PHONE;
        private string BOTPASSWORD;

        public static async Task<WrapperClient> Create(string botToken, int API_ID, string API_HASH, string PHONE, string BOTPASSWORD)
        {
            var WrapperClient = new WrapperClient(botToken, API_ID, API_HASH, PHONE, BOTPASSWORD);
            await WrapperClient.Init();
            return WrapperClient;
        }

        private WrapperClient(string botToken, int API_ID, string API_HASH, string PHONE, string BOTPASSWORD)
        {
            this.botToken = botToken;
            this.API_ID = API_ID;
            this.API_HASH = API_HASH;
            this.PHONE = PHONE;
            this.BOTPASSWORD = BOTPASSWORD;
        }

        private async Task Init()
        {
            this.client = client = new WTelegram.Client(API_ID, API_HASH); ;

            await DoLogin(PHONE);
        }

        async Task DoLogin(string loginInfo)
        {
            while (this.client.User == null)

                switch (await client.Login(loginInfo)) // returns which config info is needed to continue login
                {
                    case "verification_code": loginInfo = await ReceiveBotMessage(); break;
                    case "name": loginInfo = "John Doe"; break;    // if sign-up is required (first_name last_name)
                    case "password": loginInfo = "password"; break; // if user has enabled 2FA
                    default: loginInfo = null; break;
                }

        }

        public async Task<string> SendMessageToUser(string user, string message, bool phone = false)
        {
            try
            {

                // If a phone number was provided
                if (phone)
                {

                    TL.User contact;

                    var contacts = await client.Contacts_ImportContacts(new[] { new InputPhoneContact { phone = user } });

                    if (contacts.imported.Length > 0)
                        contact = contacts.users[contacts.imported[0].user_id];

                    else
                        return $"Error while sending Message! (Problem finding User)";

                    await client.SendMessageAsync(contact, message);

                    return $"Message: \"{message}\" sent successfully to \"{user}\"";

                }

                // If a user name was provided
                else
                {
                    var contact = await client.Contacts_ResolveUsername(user);

                    await client.SendMessageAsync(contact, message);

                    return $"Message: \"{message}\" sent successfully to \"{user}\"";
                }

            }

            catch (Exception e)
            {
                return $"Error while sending Message! {e.Message}";
            }

        }

        public async Task<string> ReceiveBotMessage()
        {
            string result;
            int seconds = 0;

            CancellationTokenSource cts = new CancellationTokenSource();

            // Cancel automatically after five minutes

            var bot = new TelegramBot(botToken, BOTPASSWORD, cts);
            bot.StartBot();

            // Check for new Code every second, cancel automatically after five minutes
            while (bot.GetCode() == null && seconds < 60 * 5)
            {
                await Task.Delay(1000);
                seconds++;
            }

            result = bot.GetCode();

            cts.Cancel();

            return result;

        }
    }
}
