﻿using Lexi.Core.Api.Models.Foundations.Users;
using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Lexi.Core.Api.Services.Foundations.TelegramHandles
{
    public partial class TelegramHandleService
    {
        private async ValueTask<bool> Me(
            ITelegramBotClient client,
            Update update,
            Models.Foundations.Users.User user)
        {
            if (user.State is State.Settigns && update.Message.Text is "Me 👤")
            {
                decimal? originalValue = user.Overall;
                int decimalPlaces = 1;

                decimal? roundedValue = originalValue.HasValue ? Math.Round(originalValue.Value, decimalPlaces) : 0;

                await client.SendTextMessageAsync(
                   chatId: update.Message.Chat.Id,
                   replyMarkup: OptionMarkup(),
                   text: $"About me👤\n\nName: {user.Name}\nLevel: {user.Level}\nAverage pronunciation result: {roundedValue}% 🧠");

                user.State = State.Me;
                await this.updateStorageBroker.UpdateUserAsync(user);

                return true;
            }
            else if (user.State is State.Me && update.Message.Text is "Change my English level \U0001f92f")
            {
                await client.SendTextMessageAsync(
                       chatId: update.Message.Chat.Id,
                       replyMarkup: LevelMarkup(),
                       text: $"Choose your current English level: 👇🏼");

                user.State = State.ChangeLevel;
                await this.updateStorageBroker.UpdateUserAsync(user);

                return true;
            }
            else if (user.State is State.ChangeLevel)
            {
                await client.SendTextMessageAsync(
                       chatId: update.Message.Chat.Id,
                       replyMarkup: MenuMarkup(),
                       text: $"Changed 👍🏼");

                user.State = State.Active;
                user.Level = update.Message.Text;
                await this.updateStorageBroker.UpdateUserAsync(user);

                return true;
            }

            else if (user.State is State.Me && update.Message.Text is "Change English level \U0001f92f")
            {
                await client.SendTextMessageAsync(
                   chatId: update.Message.Chat.Id,
                   replyMarkup: OptionMarkup(),
                   text: $"Choose 👇🏼");

                return true;
            }

            return false;
        }
    }
}
