﻿using Lexi.Core.Api.Models.Foundations.ExternalUsers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Lexi.Core.Api.Brokers.TelegramBroker
{
    public interface ITelegramBroker
    {
        ValueTask<ExternalUser> CreateExternalUserAsync();
        void StartListening();
        void ReturningConvertOggToWav(Stream stream);
        string ReturnFilePath();
        Task SendTextMessageAsync(long chatId, string text);
    }
}