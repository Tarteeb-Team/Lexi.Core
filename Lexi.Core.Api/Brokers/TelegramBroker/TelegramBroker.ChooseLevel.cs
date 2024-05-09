using Lexi.Core.Api.Models.Foundations.Users;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Lexi.Core.Api.Brokers.TelegramBroker
{
    public partial class TelegramBroker
    {
        private async ValueTask<bool> ChooseLevel(
        ITelegramBotClient client,
        Update update,
        Models.Foundations.Users.User user)
        {
            if (user.State is State.Level)
            {
                if (update.Message.Text is "A1 😊"
                || update.Message.Text is "A2 😉"
                || update.Message.Text is "B1 😄"
                || update.Message.Text is "B2 😎"
                || update.Message.Text is "C1 😇"
                || update.Message.Text is "C2 🤗")
                {
                    storedLevel.Value = update.Message.Text;
                    await client.SendTextMessageAsync(
                       chatId: update.Message.Chat.Id,
                       replyMarkup: MenuMarkup(),
                       text: $"Welcome {user.Name} ⚡️\n\nYour level is {update.Message.Text} ⭐️\n\nChoose 👇🏼");

                    user.State = State.Active;
                    user.Level = update.Message.Text;
                    await this.updateStorageBroker.UpdateUserAsync(user);

                    return true;
                }

                await client.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    text: "Please, choose your level ❗️");

                return true;
            }

            return false;
        }
    }
}
