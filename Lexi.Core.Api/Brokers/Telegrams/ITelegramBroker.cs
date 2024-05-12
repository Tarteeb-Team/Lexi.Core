using System.Threading.Tasks;
using System;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace Lexi.Core.Api.Brokers.Telegrams
{
    public interface ITelegramBroker
    {
        void RegisterTelegramEventHandler(Func<Update, ITelegramBotClient, ValueTask> eventHandler);
    }
}
