//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Lexi.Core.Api.Services.Foundations.Telegrams
{
    public class TelegramService : ITelegramService
    {
        private TelegramBotClient botClient;
        public TelegramService()
        {
            var token = "6505501647:AAEefapD-rEHaoFw6gyG-UNbI3KCCm6NxKU";
            this.botClient = new TelegramBotClient(token);
        }

        public void StartListening()
        {
            botClient.StartReceiving(MessageHandler, ErrorHandler);
        }

        private async Task MessageHandler(ITelegramBotClient client, Update update, CancellationToken token)
        {
            if(update.Message.Text is not null)
            {
                await this.botClient.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    text: "Hi mario!");
            }
        }

        static async Task ErrorHandler(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
        }
    }
}
