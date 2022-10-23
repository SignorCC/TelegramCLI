using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegram_CLI
{
    public class TelegramBot
    {
        public ITelegramBotClient botClient;
        private string password;
        private string code;
        private CancellationTokenSource cts;

        public TelegramBot(string botToken, string password, CancellationTokenSource cancellationToken)
        {
            botClient = new TelegramBotClient(botToken);
            this.password = password;
            this.code = "void";
            this.cts = cancellationToken;
        }

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Message is not { } message)
                return;
            

            // Only process text messages
            if (message.Text is not { } messageText)
                return;

            // Check for null
            if (update.Message.Text == null)
                return;

            var chatId = message.Chat.Id;

            // Pass received Message to Handler
            string text = update.Message.Text;
            {
                if (!text.StartsWith("/" + password))
                {
                    await botClient.SendTextMessageAsync(chatId, "Error while parsing 2FA Code!");
                    return;
                }

                else
                {
                    // Return 2FA Code
                    string[] parts = text.Split(' ');
                    int code = 0;

                    if (parts.Length > 1 && int.TryParse(parts[1], out code))
                    {
                        await botClient.SendTextMessageAsync(chatId, $"2FA Code {code} parsed successfully!");

                        this.code = parts[1];
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(chatId, "Error while parsing 2FA Code!");
                        return;
                    }
                }
            }
        }

        Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                // If API Error
                ApiRequestException apiRequestException

                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",

                // If not (default)
                _ => exception.ToString()
            };

            return Task.CompletedTask;
        }

        public async Task StartBot()
        {
            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
            };

            this.botClient.StartReceiving
            (
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: this.cts.Token
            ) ;
        }

        public string GetCode()
        {
            if (this.code == "void")
                return null;

            return this.code;
        }


    }
}
