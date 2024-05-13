using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Threading;
using System;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Lexi.Core.Api.Brokers.Telegrams
{
    public class TelegramBroker : ITelegramBroker
    {
        private readonly TelegramBotClient telegramBotClient;
        private static Func<Update, ITelegramBotClient, ValueTask> taskHandler;

        public TelegramBroker(IConfiguration configuration)
        {
            var token = "6908660319:AAE5I0sDaBLp5P5nm1Kf1ywdl7LmZXC-kqQ";
            this.telegramBotClient = new TelegramBotClient(token);
            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };

            this.telegramBotClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions);
        }

        public TelegramBotClient ReturnTelegramBotClient() =>
            this.telegramBotClient;

        private async Task HandleUpdateAsync(ITelegramBotClient telegramBotClient, Update update, CancellationToken ct)
        {
            try
            {
                Task.Run(async () => await taskHandler(update, telegramBotClient));
            }
            catch (Exception ex)
            {
                Console.Write($"{ex}");
            }
        }

        public void RegisterTelegramEventHandler(Func<Update, ITelegramBotClient, ValueTask> eventHandler) =>
            taskHandler = eventHandler;

        private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}
